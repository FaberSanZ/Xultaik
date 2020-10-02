// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ShaderBytecode.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zeckoxe.ShaderCompiler;

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

    public class ShaderBytecode
    {
        public ShaderBytecode(string path, ShaderStage stage)
        {
            Data = Compiler.LoadFromFile(path, ToStage(stage));
            Stage = stage;
        }

        public ShaderBytecode(byte[] buffer, ShaderStage stage)
        {
            Data = buffer;
            Stage = stage;
        }


        public byte[] Data { get; set; }

        public ShaderStage Stage { get; set; }

        // TODO: ToStage
        private static int ToStage(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return 0;

                case ShaderStage.Fragment:
                    return 1;

                case ShaderStage.Compute:
                    return 0;

                case ShaderStage.Geometry:
                    return 0;

                case ShaderStage.TessellationControl:
                    return 0;

                case ShaderStage.TessellationEvaluation:
                    return 0;

                case ShaderStage.RaygenKHR:
                    return 0;

                case ShaderStage.AnyHitKHR:
                    return 0;

                case ShaderStage.ClosestHitKHR:
                    return 0;

                case ShaderStage.MissKHR:
                    return 0;

                case ShaderStage.IntersectionKHR:
                    return 0;

                case ShaderStage.CallableKHR:
                    return 0;

                case ShaderStage.TaskNV:
                    return 0;

                case ShaderStage.MeshNV:
                    return 0;

                default:
                    return 0;

            }
        }


        public static ShaderBytecode LoadFromFile(string path, ShaderStage stage) => new ShaderBytecode(path, stage);
        public static ShaderBytecode LoadFromFile(byte[] bytes, ShaderStage stage) => new ShaderBytecode(bytes, stage);



        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
