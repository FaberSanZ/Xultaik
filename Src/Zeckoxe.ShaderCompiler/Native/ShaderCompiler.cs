// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	ShadercNative.cs
=============================================================================*/



using System;

namespace Zeckoxe.ShaderCompiler
{
    internal unsafe class ShaderCompiler : IDisposable
    {
        internal enum Stage
        {
            Vertex,

            Fragment,

            Compute,

            Geometry,

            TessControl,

            TessEvaluation,



            GLSL_Vertex = Vertex,
            GLSL_Fragment = Fragment,
            GLSL_Compute = Compute,
            GLSL_Geometry = Geometry,
            GLSL_TessControl = TessControl,
            GLSL_TessEvaluation = TessEvaluation,
            GLSL_infer_from_source,
            GLSL_DefaultVertex,
            GLSL_DefaultFragment,
            GLSL_DefaultCompute,
            GLSL_DefaultGeometry,
            GLSL_DefaultTess_control,
            GLSL_DefaultTess_evaluation,
            SpirV_Assembly,



            Raygen,

            Anyhit,

            Closesthit,

            Miss,

            Intersection,

            Callable,


            GLSL_Raygen = Raygen,
            GLSL_Anyhit = Anyhit,
            GLSL_Closesthit = Closesthit,
            GLSL_Miss = Miss,
            GLSL_Intersection = Intersection,
            GLSL_Callable = Callable,
            GLSL_Default_Raygen,
            GLSL_Default_Anyhit,
            GLSL_Default_Closesthit,
            GLSL_Default_Miss,
            GLSL_Default_Intersection,
            GLSL_Default_Callable,


            Task,

            Mesh,



        }

        private readonly IntPtr _handle;

        internal IntPtr NativeHandle => _handle;

        internal ShaderCompiler()
        {
            _handle = ShadercNative.Initialize();
        }





        internal CompileResult Compile(string source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            IntPtr resultPtr = ShadercNative.CompileIntoSPV(_handle, Core.Interop.String.ToPointer(source), new UIntPtr((uint)source.Length), (int)stage, Zeckoxe.Core.Interop.String.ToPointer(name), Core.Interop.String.ToPointer(entryPoint), options.NativeHandle);
            return new CompileResult(resultPtr);
        }




        internal CompileResult Preprocess(string source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            IntPtr resultPtr = ShadercNative.CompileIntoPreprocessedText(_handle, Core.Interop.String.ToPointer(source + source),
                                            new UIntPtr((uint)source.Length), (int)stage, Core.Interop.String.ToPointer(name),
                                            Core.Interop.String.ToPointer(entryPoint), options.NativeHandle);
            return new CompileResult(resultPtr);
        }

        public void Dispose()
        {
            ShadercNative.Release(_handle);
        }
    }
}
