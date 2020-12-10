// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ShaderBytecode.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shaderc;

namespace Zeckoxe.Graphics
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

            Options _options = new(false)
            {
                SourceLanguage = SourceLanguage.Glsl,
                InvertY = options.InvertY,
                //Optimization = OptimizationLevel.Performance,
            };


            using Compiler compiler = new(_options);

            Result result = compiler.Compile(path, StageToShaderKind(stage));

            Data = result.GetData();



        }

        public ShaderBytecode(byte[] buffer, ShaderStage stage)
        {
            Data = buffer;
            Stage = stage;
        }


        public byte[] Data { get; set; }

        public ShaderStage Stage { get; set; }

        // TODO: ToStage
        private static ShaderKind StageToShaderKind(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return ShaderKind.VertexShader;

                case ShaderStage.Fragment:
                    return ShaderKind.FragmentShader;

                case ShaderStage.Compute:
                    return ShaderKind.ComputeShader;

                case ShaderStage.Geometry:
                    return ShaderKind.GeometryShader;

                case ShaderStage.TessellationControl:
                    return ShaderKind.TessControlShader;

                case ShaderStage.TessellationEvaluation:
                    return ShaderKind.TessEvaluationShader;

                case ShaderStage.RaygenKHR:
                    return ShaderKind.RaygenShader;

                case ShaderStage.AnyHitKHR:
                    return ShaderKind.AnyhitShader;

                case ShaderStage.ClosestHitKHR:
                    return ShaderKind.ClosesthitShader;

                case ShaderStage.MissKHR:
                    return ShaderKind.MissShader;

                case ShaderStage.IntersectionKHR:
                    return ShaderKind.IntersectionShader;

                case ShaderStage.CallableKHR:
                    return ShaderKind.CallableShader;

                case ShaderStage.TaskNV:
                    return ShaderKind.TaskShader;

                case ShaderStage.MeshNV:
                    return ShaderKind.MeshShader;

                default:
                    return 0;

            }
        }


        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage) => new ShaderBytecode(path, stage);
        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage) => new ShaderBytecode(bytes, stage);



        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
