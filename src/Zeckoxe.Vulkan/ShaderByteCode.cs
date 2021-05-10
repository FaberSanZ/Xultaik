// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using SPIRVCross;
using static SPIRVCross.SPIRV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vortice.Dxc;
using Vortice.ShaderCompiler;

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
    }

    public unsafe class ShaderBytecode
    {

        private Vortice.ShaderCompiler.Result result;

        public ShaderBytecode(string path, ShaderStage stage,  bool ishlsl = false, ShaderBytecodeOptions options = default)
        {
            Stage = stage;

            if (!ishlsl)
            {
                if (options == default)
                    options = new() { InvertY = false, };

                Options _options = new();

                _options.SetSourceLanguage(SourceLanguage.GLSL);
                //_options.SetInvertY(true);


                using Vortice.ShaderCompiler.Compiler compiler = new(_options);

                result = compiler.Compile(File.ReadAllText(path), string.Empty, stage.StageToShaderKind());

                Data = result.GetBytecode().ToArray();
            }
            else
            {
                var profile = "";
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

        public ShaderBytecode(byte[] buffer, ShaderStage stage)
        {
            Data = buffer;
            Stage = stage;
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

        // TODO: ToStage



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

            var compiler = Dxc.CreateDxcCompiler3();

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
                spirv = (SpvId*)ptr;

            uint word_count = (uint)data.Length / 4;



            spvc_context context = default;
            spvc_parsed_ir ir;
            spvc_compiler compiler_glsl;
            spvc_compiler_options options;
            spvc_resources resources;
            spvc_reflected_resource* list = default;
            byte* result_ = null;
            nuint count = default;
            spvc_error_callback error_callback = default;


            // Create context.
            spvc_context_create(&context);

            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);

            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_context_create_compiler(context, spvc_backend.Hlsl, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

            // Do some basic reflection.
            spvc_compiler_create_shader_resources(compiler_glsl, &resources);
            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.UniformBuffer, (spvc_reflected_resource*)&list, &count);


            var model = spvc_compiler_get_execution_model(compiler_glsl);

            Console.WriteLine(model);

            for (uint i = 0; i < count; i++)
            {
                //Console.WriteLine("ID: {0}, BaseTypeID: {1}, TypeID: {2}, Name: {3}", list[i].id, list[i].base_type_id, list[i].type_id, GetString(list[i].name));

                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                Console.WriteLine($"Set: {set}");

                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationBinding);
                Console.WriteLine($"Binding: {binding}");


                Console.WriteLine("=========");


                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.UniformBuffer,
                    stage = Stage,
                });
            }
            Console.WriteLine("\n \n");
            //spvc_

            // Modify options.
            //spvc_compiler_create_compiler_options(compiler_glsl, &options);
            //spvc_compiler_options_set_uint(options, spvc_compiler_option.GlslVersion, 450);
            //spvc_compiler_options_set_bool(options, spvc_compiler_option.GlslEs, false);
            //spvc_compiler_install_compiler_options(compiler_glsl, options);


            //byte* r = default;
            //spvc_compiler_compile(compiler_glsl, (byte*)&r);
            //Console.WriteLine("Cross-compiled source: {0}", GetString(r));

            // Frees all memory we allocated so far.
            spvc_context_destroy(context);

            //using (Context context = new Context())
            //{
            //    ParseIr ir = context.ParseIr(data);
            //    SharpSPIRVCross.Compiler compiler = context.CreateCompiler(Backend.HLSL, ir);

            //    ShaderResources resources = compiler.CreateShaderResources();

            //    foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.PushConstant))
            //    {
            //        uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
            //        uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);
            //        uint offset = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Offset);
            //        int size = 0;
            //        SpirvType type = compiler.GetSpirvType(uniformBuffer.TypeId);
            //        compiler.GetDeclaredStructSize(type, out size);

            //        Resources.Add(new()
            //        {
            //            size = (uint)size,
            //            set = set,
            //            binding = binding,
            //            resource_type = ResourceType.PushConstant,
            //            offset = offset,
            //            stage = Stage,
            //        });
            //    }

            //    foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.SubpassInput))
            //    {
            //        uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
            //        uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);

            //        Resources.Add(new()
            //        {
            //            set = set,
            //            binding = binding,
            //            resource_type = ResourceType.SubpassInput,
            //            stage = Stage,
            //        });
            //    }


            //    foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.UniformBuffer))
            //    {
            //        uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
            //        uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);

            //        Resources.Add(new()
            //        {
            //            set = set,
            //            binding = binding,
            //            resource_type = ResourceType.UniformBuffer,
            //            stage = Stage,
            //        });
            //    }

            //    //foreach (ReflectedResource input in resources.GetResources(ResourceType.StageInput))
            //    //{
            //    //    uint location = compiler.GetDecoration(input.Id, SpvDecoration.Location);

            //    //    Resources.Add(new()
            //    //    {
            //    //        location = location,
            //    //        resource_type = ResourceType.StageInput,
            //    //        stage = Stage,
            //    //    });
            //    //}


            //    foreach (ReflectedResource separateImage in resources.GetResources(ResourceType.SeparateImage))
            //    {
            //        uint set = compiler.GetDecoration(separateImage.Id, SpvDecoration.DescriptorSet);
            //        uint binding = compiler.GetDecoration(separateImage.Id, SpvDecoration.Binding);
            //        //uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.);

            //        Resources.Add(new()
            //        {
            //            set = set,
            //            binding = binding,
            //            resource_type = ResourceType.SeparateImage,
            //            stage = Stage,
            //        });
            //    }


            //    foreach (ReflectedResource sampledImage in resources.GetResources(ResourceType.SampledImage))
            //    {
            //        uint set = compiler.GetDecoration(sampledImage.Id, SpvDecoration.DescriptorSet);
            //        uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.Binding);
            //        //uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.);

            //        Resources.Add(new()
            //        {
            //            set = set,
            //            binding = binding,
            //            resource_type = ResourceType.SampledImage,
            //            stage = Stage,
            //        });
            //    }

            //}
        }

        //public byte* GetBytes()
        //{
        //    return result.GetBytes();
        //}

        //public Span<byte> GetBytecode()
        //{
        //    return result.GetBytecode();
        //}
        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage) => new ShaderBytecode(path, stage);
        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage) => new ShaderBytecode(bytes, stage);



        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
