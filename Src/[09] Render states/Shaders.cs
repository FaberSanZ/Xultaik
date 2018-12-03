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

namespace _09__Render_states
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

        ///////////////**************new**************////////////////////
        public static Buffer CreateBuffer<T>(Device device) where T : struct
        {
            return new Buffer(device, Utilities.SizeOf<T>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }
}
