using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace _12___Iluminación_Difusa
{

    public static class ShadersUtil
    {
        public static ShaderBytecode CompileShader(string FileName, string EntryPoint, string Profile, ShaderMacro[] defines = null)
        {
            ShaderFlags shaderFlags = ShaderFlags.None;

            shaderFlags |= ShaderFlags.Debug | ShaderFlags.SkipOptimization;

            CompilationResult result = ShaderBytecode.CompileFromFile(FileName, EntryPoint, Profile, shaderFlags, defines: defines);

            return new ShaderBytecode(result);
        }
    }
}
