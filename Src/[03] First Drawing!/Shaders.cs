using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;

namespace _03__First_Drawing_
{
    public static class Shaders
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
