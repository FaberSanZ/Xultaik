// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ShaderBytecode.cs
=============================================================================*/


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


    public class ShaderBytecodeOptions
    {
        public bool InvertY { get; set; }
    }

    public class ShaderBytecode
    {
        public ShaderBytecode(string path, ShaderStage stage, ShaderBytecodeOptions options = default)
        {
            Stage = stage;


            if (options == default)
                options = new() { InvertY = false, };

            Options _options = new()
            {
                
                //SourceLanguage = Shaderc.SourceLanguage.Glsl,
                //InvertY = options.InvertY,
                ////Optimization = OptimizationLevel.Performance,
            };

            _options.SetSourceLanguage(SourceLanguage.GLSL);
            //_options.SetInvertY(true);


            using Compiler compiler = new(_options);

            result = compiler.Compile(File.ReadAllText(path), string.Empty, stage.StageToShaderKind());

            Data = result.GetBytecode().ToArray();
        }

        public ShaderBytecode(byte[] buffer, ShaderStage stage)
        {
            Data = buffer;
            Stage = stage;
        }


        public ShaderBytecode(Span<byte> buffer, ShaderStage stage)
        {
            Data = buffer.ToArray();
            Stage = stage;
        }


        public unsafe byte* GetBytes()
        {
            return result.GetBytes();
        }

        public Span<byte> GetBytecode()
        {
            return result.GetBytecode();
        }

        private Result result;

        public byte[] Data { get; set; }
        public ShaderStage Stage { get; set; }

        // TODO: ToStage
        


        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage) => new ShaderBytecode(path, stage);
        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage) => new ShaderBytecode(bytes, stage);



        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
