// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ShaderBytecode.cs
=============================================================================*/


using SharpSPIRVCross;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        internal ResourceType resource_type { get; set; }
        public ShaderStage stage { get; set; }
    }

    public class ShaderBytecodeOptions
    {
        public bool InvertY { get; set; }
    }

    public class ShaderBytecode
    {

        private Vortice.ShaderCompiler.Result result;

        public ShaderBytecode(string path, ShaderStage stage, ShaderBytecodeOptions options = default)
        {
            Stage = stage;


            if (options == default)
                options = new() { InvertY = false, };

            Options _options = new();

            _options.SetSourceLanguage(SourceLanguage.GLSL);
            //_options.SetInvertY(true);


            using Vortice.ShaderCompiler.Compiler compiler = new(_options);

            result = compiler.Compile(File.ReadAllText(path), string.Empty, stage.StageToShaderKind());

            Data = result.GetBytecode().ToArray();

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

        public void AddShaderResource(byte[] data)
        {
            using (Context context = new Context())
            {
                ParseIr ir = context.ParseIr(data);
                SharpSPIRVCross.Compiler compiler = context.CreateCompiler(Backend.GLSL, ir);

                ShaderResources resources = compiler.CreateShaderResources();

                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.PushConstant))
                {
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);
                    uint offset = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Offset);
                    int size = 0;
                    SpirvType type = compiler.GetSpirvType(uniformBuffer.TypeId);
                    compiler.GetDeclaredStructSize(type, out size);

                    Resources.Add(new()
                    {
                        size = (uint)size,
                        set = set,
                        binding = binding,
                        resource_type = ResourceType.PushConstant,
                        offset = offset,
                        stage = Stage,
                    });
                }

                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.SubpassInput))
                {
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);

                    Resources.Add(new()
                    {
                        set = set,
                        binding = binding,
                        resource_type = ResourceType.SubpassInput,
                        stage = Stage,
                    });
                }


                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.UniformBuffer))
                {
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);

                    Resources.Add(new()
                    {
                        set = set,
                        binding = binding,
                        resource_type = ResourceType.UniformBuffer,
                        stage = Stage,
                    });
                }

                //foreach (ReflectedResource input in resources.GetResources(ResourceType.StageInput))
                //{
                //    uint location = compiler.GetDecoration(input.Id, SpvDecoration.Location);

                //    Resources.Add(new()
                //    {
                //        location = location,
                //        resource_type = ResourceType.StageInput,
                //        stage = Stage,
                //    });
                //}


                foreach (ReflectedResource separateImage in resources.GetResources(ResourceType.SeparateImage))
                {
                    uint set = compiler.GetDecoration(separateImage.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(separateImage.Id, SpvDecoration.Binding);
                    //uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.);

                    Resources.Add(new()
                    {
                        set = set,
                        binding = binding,
                        resource_type = ResourceType.SeparateImage,
                        stage = Stage,
                    });
                }


                foreach (ReflectedResource sampledImage in resources.GetResources(ResourceType.SampledImage))
                {
                    uint set = compiler.GetDecoration(sampledImage.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.Binding);
                    //uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.);

                    Resources.Add(new()
                    {
                        set = set,
                        binding = binding,
                        resource_type = ResourceType.SampledImage,
                        stage = Stage,
                    });
                }

            }
        }

        public unsafe byte* GetBytes()
        {
            return result.GetBytes();
        }

        public Span<byte> GetBytecode()
        {
            return result.GetBytecode();
        }
        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage) => new ShaderBytecode(path, stage);
        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage) => new ShaderBytecode(bytes, stage);



        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
