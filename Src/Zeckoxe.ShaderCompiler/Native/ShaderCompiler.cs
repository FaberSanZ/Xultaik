using System;
using System.IO;
using System.Runtime.InteropServices;
using Zeckoxe.Core;

namespace Zeckoxe.ShaderCompiler
{
    internal unsafe class ShaderCompiler
    {
        internal enum Stage 
        {

            Vertex,

            Fragment,

            Compute,

            Geometry,

            TessControl,

            TessEvaluation,



            Shaderc_GLSL_Vertex = Vertex,
            Shaderc_GLSL_Fragment = Fragment,
            Shaderc_GLSL_Compute = Compute,
            Shaderc_GLSL_Geometry = Geometry,
            Shaderc_GLSL_TessControl = TessControl,
            Shaderc_GLSL_TessEvaluation = TessEvaluation,
            Shaderc_GLSL_infer_from_source,
            Shaderc_GLSL_DefaultVertex,
            Shaderc_GLSL_DefaultFragment,
            Shaderc_GLSL_DefaultCompute,
            Shaderc_GLSL_DefaultGeometry,
            Shaderc_GLSL_DefaultTess_control,
            Shaderc_GLSL_DefaultTess_evaluation,
            shaderc_SpirV_Assembly,



            Raygen,

            Anyhit,

            Closesthit,

            Miss,

            Intersection,

            Callable,


            Shaderc_GLSL_Raygen = Raygen,
            Shaderc_GLSL_Anyhit = Anyhit,
            Shaderc_GLSL_Closesthit = Closesthit,
            Shaderc_GLSL_Miss = Miss,
            Shaderc_GLSL_Intersection = Intersection,
            Shaderc_GLSL_Callable = Callable,
            Shaderc_GLSL_Default_Raygen,
            Shaderc_GLSL_Default_Anyhit,
            Shaderc_GLSL_Default_Closesthit,
            Shaderc_GLSL_Default_Miss,
            Shaderc_GLSL_Default_Intersection,
            Shaderc_GLSL_Default_Callable,


            Task,

            Mesh,


 
        }

        private IntPtr _handle;

        internal IntPtr NativeHandle => _handle;

        internal ShaderCompiler()
        {
            _handle = ShadercNative.shaderc_compiler_initialize();
        }

        ~ShaderCompiler()
        {
            ShadercNative.shaderc_compiler_release(_handle);
        }




        internal CompileResult Compile(string source, Stage stage , CompileOptions options, string name,string entryPoint= "main")
        {
            IntPtr resultPtr = ShadercNative.shaderc_compile_into_spv(_handle, Interop.String.ToPointer(source), new UIntPtr((uint)source.Length), (int)stage, Interop.String.ToPointer(name), Interop.String.ToPointer(entryPoint), options.NativeHandle);
            return new CompileResult(resultPtr);
        }




        internal CompileResult Preprocess(string source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            IntPtr resultPtr = ShadercNative.shaderc_compile_into_preprocessed_text(_handle, source, new UIntPtr((uint) source.Length), (int) stage, name, entryPoint, options.NativeHandle);
            return new CompileResult(resultPtr);
        }

    }
}
