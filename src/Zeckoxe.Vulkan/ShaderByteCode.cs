// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using SPIRVCross;
using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Dxc;
using Vortice.ShaderCompiler;
using static SPIRVCross.SPIRV;

namespace Zeckoxe.Vulkan
{
    // TODO: ShaderStage
    public enum ShaderStage
    {
        None = 0,
        Vertex = 1,
        TessellationControl = 2,
        TessellationEvaluation = 4,
        Geometry = 8,
        Fragment = 16,
        AllGraphics = 31,
        Compute = 32,
        TaskNV = 64,
        MeshNV = 128,
        RaygenKHR = 256,
        RaygenNV = 256,
        AnyHitKHR = 512,
        AnyHitNV = 512,
        ClosestHitKHR = 1024,
        ClosestHitNV = 1024,
        MissKHR = 2048,
        MissNV = 2048,
        IntersectionKHR = 4096,
        IntersectionNV = 4096,
        CallableKHR = 8192,
        CallableNV = 8192,
        All = int.MaxValue
    }

    public enum ShaderBackend
    {
        None = 0,
        Glsl = 1,
        Hlsl = 2,
    }

    internal class ShaderResource
    {
        internal uint offset { get; set; }

        internal uint set { get; set; }
        internal uint binding { get; set; }
        internal uint size { get; set; }
        internal uint location { get; set; }
        internal spvc_resource_type resource_type { get; set; }
        public ShaderStage stage { get; set; }
    }

    public class ShaderBytecodeOptions
    {
        public bool InvertY { get; set; }
        public ShaderBackend Backend { get; set; }
    }

    public unsafe class ShaderBytecode
    {

        private Vortice.ShaderCompiler.Result result;
        public ShaderBytecode(string path, ShaderStage stage, ShaderBytecodeOptions options = default)
        {
            Stage = stage;
            Options = options;
            if (options.Backend == ShaderBackend.Glsl)
            {
                if (options == default)
                {
                    options = new() 
                    { 
                        InvertY = false, 
                        Backend = ShaderBackend.Glsl
                    };
                }

                Options _options = new();

                _options.SetSourceLanguage(SourceLanguage.GLSL);
                //_options.SetInvertY(true);


                using Vortice.ShaderCompiler.Compiler compiler = new(_options);

                result = compiler.Compile(File.ReadAllText(path), string.Empty, stage.StageToShaderKind());

                Data = result.GetBytecode().ToArray();
            }
            else if(options.Backend == ShaderBackend.Hlsl)
            {
                string? profile = "";
                if (stage == ShaderStage.Vertex)
                {
                    profile = "vs_6_0";
                }

                else if (stage == ShaderStage.Fragment)
                {
                    profile = "ps_6_0";
                }

                Data = CompileHLSL(path, profile);

            }



            AddShaderResource(Data);
        }

        public ShaderBytecode(byte[] buffer, ShaderStage stage, ShaderBackend shaderBackend)
        {
            Data = buffer;
            Stage = stage;
            Options = new()
            {
                InvertY = false,
                Backend = shaderBackend
            };
            AddShaderResource(buffer);

        }


        public ShaderBytecode(Span<byte> buffer, ShaderStage stage)
        {
            Data = buffer.ToArray();
            Stage = stage;
            AddShaderResource(buffer.ToArray());

        }




        internal List<ShaderResource> Resources { get; set; } = new();

        public byte[] Data { get; set; }
        public ShaderStage Stage { get; set; }
        public ShaderBytecodeOptions Options { get; set; }



        private byte[] CompileHLSL(string filePath, string profile = "vs_6_0")
        {
            string? source = File.ReadAllText(filePath);
            string[] args = new[]
            {
                "-spirv",
                "-T", profile,
                "-E", "main",
                "-fspv-target-env=vulkan1.2",
                "-fspv-extension=SPV_NV_ray_tracing",
                "-fspv-extension=SPV_KHR_multiview",
                "-fspv-extension=SPV_KHR_shader_draw_parameters",
                "-fspv-extension=SPV_EXT_descriptor_indexing",

            };
            IDxcUtils? utils = Dxc.CreateDxcUtils();
            IDxcIncludeHandler? handler = utils!.CreateDefaultIncludeHandler();

            IDxcCompiler3? compiler = Dxc.CreateDxcCompiler3();

            IDxcResult? result = compiler?.Compile(source, args, handler);

            if (result == null || result.GetStatus().Failure)
            {
                throw new Exception(result!.GetErrors());
            }

            byte[] data = result.GetObjectBytecodeArray();

            result.Dispose();
            return data;
        }

        public void AddShaderResource(byte[] data)
        {

            SpvId* spirv;

            fixed (byte* ptr = data)
            {
                spirv = (SpvId*)ptr;
            }

            uint word_count = (uint)data.Length / 4;



            spvc_context context = default;
            spvc_parsed_ir ir;
            spvc_compiler compiler_glsl;
            spvc_compiler_options options;
            spvc_resources resources;


            spvc_reflected_resource* uniformBufferList = default;
            nuint uniformBufferCount = default;

            spvc_reflected_resource* separateImageList = default;
            nuint separateImageCount = default;

            spvc_reflected_resource* sampledImageList = default;
            nuint sampledImageCount = default;

            spvc_reflected_resource* pushConstantList = default;
            nuint pushConstantCount = default;



            byte* result_ = null;
            spvc_error_callback error_callback = default;


            // Create context.
            spvc_context_create(&context);

            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);

            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_backend backend = Options.Backend == ShaderBackend.Hlsl ? spvc_backend.Hlsl : spvc_backend.Glsl;
            spvc_context_create_compiler(context, backend, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

            spvc_compiler_create_shader_resources(compiler_glsl, &resources);


            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.UniformBuffer, (spvc_reflected_resource*)&uniformBufferList, &uniformBufferCount);
            for (uint i = 0; i < uniformBufferCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)uniformBufferList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)uniformBufferList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.UniformBuffer,
                    stage = Stage,
                });
            }



            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SeparateImage, (spvc_reflected_resource*)&separateImageList, &separateImageCount);
            for (uint i = 0; i < separateImageCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)separateImageList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)separateImageList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SeparateImage,
                    stage = Stage,
                });
            }



            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SampledImage, (spvc_reflected_resource*)&sampledImageList, &sampledImageCount);
            for (uint i = 0; i < sampledImageCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)sampledImageList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)sampledImageList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SampledImage,
                    stage = Stage,
                });
            }



            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.PushConstant, (spvc_reflected_resource*)&pushConstantList, &pushConstantCount);
            for (uint i = 0; i < pushConstantCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)pushConstantList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)pushConstantList[i].id, SpvDecoration.SpvDecorationBinding);
                uint offset = spvc_compiler_get_decoration(compiler_glsl, (SpvId)pushConstantList[i].id, SpvDecoration.SpvDecorationOffset);
                spvc_type type = spvc_compiler_get_type_handle(compiler_glsl, (SpvId)pushConstantList[i].type_id);

                nuint size = 0;
                spvc_compiler_get_declared_struct_size(compiler_glsl, type, &size);

                Resources.Add(new()
                {
                    size = (uint)size,
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.PushConstant,
                    offset = offset,
                    stage = Stage,
                });
            }


            spvc_context_destroy(context);

        }


        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage)
        {
            return new ShaderBytecode(path, stage);
        }

        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage)
        {
            return new ShaderBytecode(bytes, stage);
        }

        public static implicit operator byte[](ShaderBytecode shaderBytecode)
        {
            return shaderBytecode.Data;
        }
    }
}
