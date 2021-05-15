﻿// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using SPIRVCross;
using System;
using System.Collections.Generic;
using System.IO;
using Vortice.ShaderCompiler;
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

        Fragment = 16,

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


    public unsafe class ShaderBytecode
    {

        internal List<ShaderResource> Resources { get; set; } = new();

        public ShaderBytecode(string path, ShaderStage stage)
        {
            Stage = stage;


            Data = CompileGLSL(path);



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





        public byte[] Data { get; set; }
        public ShaderStage Stage { get; set; }


        private byte[] CompileGLSL(string path)
        {
            Options _options = new();

            _options.SetSourceLanguage(SourceLanguage.GLSL);

            using Vortice.ShaderCompiler.Compiler compiler = new(_options);

            var result = compiler.Compile(File.ReadAllText(path), string.Empty, Stage.StageToShaderKind());

            return result.GetBytecode().ToArray();

        }
        //private byte[] CompileHLSL(string path)
        //{
        //    string profile = "";
        //    string? shadermodel = "_6_5";


        //    switch (Stage)
        //    {
        //        case ShaderStage.Vertex:
        //            profile = "vs" + shadermodel;
        //            break;

        //        case ShaderStage.TessellationControl:
        //            profile = "hs" + shadermodel;
        //            break;

        //        case ShaderStage.TessellationEvaluation:
        //            profile = "ds" + shadermodel;
        //            break;

        //        case ShaderStage.Geometry:
        //            profile = "gs" + shadermodel;
        //            break;

        //        case ShaderStage.Fragment:
        //            profile = "ps" + shadermodel;
        //            break;


        //        case ShaderStage.Compute:
        //            profile = "cs" + shadermodel;
        //            break;

        //        case ShaderStage.TaskNV:
        //            profile = "as_6_5";
        //            break;

        //        case ShaderStage.MeshNV:
        //            profile = "ms_6_5";
        //            break;

        //        case ShaderStage.Raygen:
        //            profile = "lib_6_5";
        //            break;


        //        case ShaderStage.AnyHit:
        //            profile = "lib_6_5";
        //            break;

        //        case ShaderStage.ClosestHit:
        //            profile = "lib_6_5";
        //            break;

        //        case ShaderStage.Miss:
        //            profile = "lib_6_5";
        //            break;

        //        case ShaderStage.Intersection:
        //            profile = "lib_6_5";
        //            break;

        //        case ShaderStage.Callable:
        //            profile = "lib_6_5";
        //            break;

        //    }




        //    string? source = File.ReadAllText(path);
        //    string[] args = new[]
        //    {
        //        "-spirv",
        //        "-T", profile,
        //        "-E", "main",
        //        "-fspv-target-env=vulkan1.1",
        //        "-fspv-extension=SPV_NV_ray_tracing",
        //        "-fspv-extension=SPV_KHR_multiview",
        //        "-fspv-extension=SPV_KHR_shader_draw_parameters",
        //        "-fspv-extension=SPV_EXT_descriptor_indexing",

        //    };
        //    IDxcUtils? utils = Dxc.CreateDxcUtils();
        //    IDxcIncludeHandler? handler = utils!.CreateDefaultIncludeHandler();

        //    IDxcCompiler3? compiler = Dxc.CreateDxcCompiler3();

        //    IDxcResult? result = compiler?.Compile(source, args, handler);

        //    if (result == null || result.GetStatus().Failure)
        //    {
        //        throw new Exception(result!.GetErrors());
        //    }

        //    byte[] data = result.GetObjectBytecodeArray();

        //    result.Dispose();
        //    return data;
        //}

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

            spvc_reflected_resource* SeparateSamplersList = default;
            nuint SeparateSamplersCount = default;




            byte* result_ = null;
            spvc_error_callback error_callback = default;


            // Create context.
            spvc_context_create(&context);

            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);

            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_context_create_compiler(context, spvc_backend.Glsl, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

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


            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.SeparateSamplers, (spvc_reflected_resource*)&SeparateSamplersList, &SeparateSamplersCount);
            for (uint i = 0; i < SeparateSamplersCount; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)SeparateSamplersList[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)SeparateSamplersList[i].id, SpvDecoration.SpvDecorationBinding);

                Resources.Add(new()
                {
                    set = set,
                    binding = binding,
                    resource_type = spvc_resource_type.SeparateSamplers,
                    stage = Stage,
                });
            }


            //Console.WriteLine(sampledImageCount);


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