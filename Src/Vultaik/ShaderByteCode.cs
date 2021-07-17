// Copyright(c) 2019 - 2021 Faber Leonardo.All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using SPIRVCross;
using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Dxc;
using static SPIRVCross.SPIRV;

namespace Vultaik
{
    public enum ShaderStage
    {
        None = 0,

        Vertex = 1,

        TessellationControl = 2,

        TessellationEvaluation = 4,

        Geometry = 8,

        Fragment = 16, // TODO: To Pixel

        AllGraphics = 31,

        Compute = 32,

        TaskNV = 64,

        MeshNV = 128,

        Raygen = 256,

        AnyHit = 512,

        ClosestHit = 1024,

        Miss = 2048,

        Intersection = 4096,

        Callable = 8192,

        All = int.MaxValue
    }

    public enum ShaderBackend
    {
        None = 0,
        Glsl = 1,
        Hlsl = 2,
        Msl = 3,
        Cpp = 4,
        Json = 5,
    }


    internal class ShaderResource
    {
        internal uint offset { get; set; }
        internal bool is_dynamic { get; set; }
        internal uint set { get; set; }
        internal bool is_array { get; set; }
        internal uint binding { get; set; }
        internal uint size { get; set; }
        internal uint location { get; set; }
        internal spvc_resource_type resource_type { get; set; }
        public ShaderStage stage { get; set; }
    }


    public unsafe class ShaderBytecode
    {

        internal List<ShaderResource> Resources { get; set; } = new();

        public ShaderBytecode(string path, ShaderStage stage, string entryPoint = "main", string directory = "")
        {
            Stage = stage;
            EntryPoint = entryPoint;
            Backend = ShaderBackend.Hlsl;
            Data = CompileHLSL(path, directory, entryPoint);




            AddShaderResource(Data);
        }

        public ShaderBytecode(byte[] buffer, ShaderStage stage, ShaderBackend backend)
        {
            Data = buffer;
            Stage = stage;
            Backend = backend;
        }


        public ShaderBytecode(Span<byte> buffer, ShaderStage stage, ShaderBackend backend)
        {
            Data = buffer.ToArray();
            Stage = stage;
            Backend = backend;
        }




        public ShaderBackend Backend { get; set; }
        public byte[] Data { get; set; }
        public ShaderStage Stage { get; set; }
        public string EntryPoint { get; set; }


        private byte[] CompileHLSL(string path, string directory, string entryPoint)
        {
            string profile = "";
            string shadermodel = "_6_5";
            bool ray_tracing = false;

            switch (Stage)
            {
                case ShaderStage.Vertex:
                    profile = "vs" + shadermodel;
                    break;

                case ShaderStage.TessellationControl:
                    profile = "hs" + shadermodel;
                    break;

                case ShaderStage.TessellationEvaluation:
                    profile = "ds" + shadermodel;
                    break;

                case ShaderStage.Geometry:
                    profile = "gs" + shadermodel;
                    break;

                case ShaderStage.Fragment:
                    profile = "ps" + shadermodel;
                    break;


                case ShaderStage.Compute:
                    profile = "cs" + shadermodel;
                    break;

                case ShaderStage.TaskNV:
                    profile = "as" + shadermodel;
                    break;

                case ShaderStage.MeshNV:
                    profile = "ms" + shadermodel;
                    break;


                case ShaderStage.Raygen:
                case ShaderStage.AnyHit:
                case ShaderStage.ClosestHit:
                case ShaderStage.Miss:
                case ShaderStage.Callable:
                case ShaderStage.Intersection:
                    profile = "lib" + shadermodel;
                    ray_tracing = true;
                    break;


            }


            List<string> args = new()
            {
                "-spirv",
                "-T",
                profile,
                "-E",
                entryPoint,
                "-fspv-target-env=vulkan1.2",
                "-fspv-extension=SPV_KHR_multiview",
                "-fspv-extension=SPV_KHR_shader_draw_parameters",
                "-fspv-extension=SPV_EXT_descriptor_indexing",
            };

            if (ray_tracing)
            {
                args.Add("SPV_KHR_ray_tracing");
            }

            string source = File.ReadAllText(path);
            using IDxcIncludeHandler includeHandler = new ShaderIncludeHandler(directory);
            IDxcCompiler3? compiler = Dxc.CreateDxcCompiler3();

            IDxcResult? result = compiler?.Compile(source, args.ToArray(), includeHandler);

            if (result is null || result.GetStatus().Failure)
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
            spvc_compiler compiler_hlsl;
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

            spvc_reflected_resource* storageImagetList = default;
            nuint storageImagetCount = default;
            

            byte* result_ = null;
            spvc_error_callback error_callback = default;


            // Create context.
            spvc_context_create(&context);

            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);

            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_backend backend = (spvc_backend)Backend;
            spvc_context_create_compiler(context, backend, ir, spvc_capture_mode.TakeOwnership, &compiler_hlsl);

            spvc_compiler_create_shader_resources(compiler_hlsl, &resources);


            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.UniformBuffer, (spvc_reflected_resource*)&uniformBufferList, &uniformBufferCount);
            for (uint i = 0; i < uniformBufferCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, uniformBufferList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, uniformBufferList[i].id, SpvDecoration.SpvDecorationBinding);

                string name = Interop.String.FromPointer(spvc_compiler_get_name(compiler_hlsl, uniformBufferList[i].id));

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.UniformBuffer,
                    stage = Stage,
                    is_dynamic = name.EndsWith("Dynamic") || name.EndsWith("dynamic")
                });
            }



            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SeparateImage, (spvc_reflected_resource*)&separateImageList, &separateImageCount);
            for (uint i = 0; i < separateImageCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, separateImageList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, separateImageList[i].id, SpvDecoration.SpvDecorationBinding);
                bool hasbind = spvc_compiler_has_decoration(compiler_hlsl, separateImageList[i].id, SpvDecoration.SpvDecorationNonUniform);
                spvc_type type = spvc_compiler_get_type_handle(compiler_hlsl, (SpvId)separateImageList[i].type_id);
                var num_array = spvc_type_get_num_array_dimensions(type);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SeparateImage,
                    stage = Stage,
                    is_array = num_array >= 0,
                });

            }


            spvc_reflected_resource* SeparateSamplersList = default;
            nuint SeparateSamplersCount = default;

            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SeparateSamplers, (spvc_reflected_resource*)&SeparateSamplersList, &SeparateSamplersCount);
            for (uint i = 0; i < SeparateSamplersCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, SeparateSamplersList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, SeparateSamplersList[i].id, SpvDecoration.SpvDecorationBinding);
                
                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SeparateSamplers,
                    stage = Stage,
                });
            }


            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SampledImage, (spvc_reflected_resource*)&sampledImageList, &sampledImageCount);
            for (uint i = 0; i < sampledImageCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, sampledImageList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, sampledImageList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SampledImage,
                    stage = Stage,
                });
            }


            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.StorageImage, (spvc_reflected_resource*)&storageImagetList, &storageImagetCount);
            for (uint i = 0; i < storageImagetCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, storageImagetList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, storageImagetList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.StorageImage,
                    stage = Stage,
                });
            }




            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.PushConstant, (spvc_reflected_resource*)&pushConstantList, &pushConstantCount);
            for (uint i = 0; i < pushConstantCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_hlsl, pushConstantList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_hlsl, pushConstantList[i].id, SpvDecoration.SpvDecorationBinding);
                uint offset = spvc_compiler_get_decoration(compiler_hlsl, pushConstantList[i].id, SpvDecoration.SpvDecorationOffset);
                spvc_type type = spvc_compiler_get_type_handle(compiler_hlsl, (SpvId)pushConstantList[i].type_id);

                nuint size = 0;
                spvc_compiler_get_declared_struct_size(compiler_hlsl, type, &size);

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


        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage, string directory = "main")
        {
            return new ShaderBytecode(path, stage, directory);
        }

        public static ShaderBytecode LoadFromBytes(byte[] bytes, ShaderStage stage, ShaderBackend backend)
        {
            return new ShaderBytecode(bytes, stage, backend);
        }

        public static implicit operator byte[](ShaderBytecode shaderBytecode)
        {
            return shaderBytecode.Data;
        }
    }
}