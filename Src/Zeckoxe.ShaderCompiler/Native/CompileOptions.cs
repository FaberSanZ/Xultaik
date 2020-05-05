using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    internal enum InputLanguage
    {
        GLSL = 0,
        HLSL = 1,
    };


    internal enum OptimizationLevel
    {
        None,
        Size,
        Performance,
    };

    internal enum Environment
    {

        Vulkan,

        OpenGL,

        OpenGLCompat,

        WebGPU,

        Default = Vulkan
    };

    internal enum IncludeType
    {
        Relative,
        Absolute
    }
    internal class CompileOptions : IDisposable
    {


        internal delegate IncludeResult IncludeHandler(string requestedSource, string requestingSource, IncludeType type);

        private IntPtr _handle;
        private InputLanguage _lang = InputLanguage.GLSL;
        private OptimizationLevel _level = OptimizationLevel.None;
        private Environment _env = Environment.Vulkan;
        private bool _debug = false;

        internal IntPtr NativeHandle => _handle;

        internal InputLanguage Language
        {
            get => _lang; 
            set
            {
                _lang = value;
                ShadercNative.CompileOptionsSetSourceLanguage(_handle, (int)_lang);
            }
        }

        internal bool GenerateDebug
        {
            get => _debug; 
            set
            {
                _debug = value;
                ShadercNative.shaderc_compile_options_set_generate_debug_info(_handle);
            }
        }

        internal OptimizationLevel Optimization
        {
            get => _level; 
            set
            {
                _level = value;
                ShadercNative.shaderc_compile_options_set_optimization_level(_handle,(int)_level);
            }
        }

        internal Environment Target
        {
            get => _env; 
            set
            {
                _env = value;
                ShadercNative.shaderc_compile_options_set_target_env(_handle, (int)_env, 0);
            }
        }


        internal CompileOptions()
        {
            _handle = ShadercNative.CompileOptionsInitialize();
        }



        public void Dispose()
        {
            ShadercNative.CompileOptionsRelease(_handle);
        }
    }
}
