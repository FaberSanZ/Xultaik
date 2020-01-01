using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    public class CompileOptions
    {
        public enum InputLanguage
        {
            GLSL = 0,
            HLSL = 1,
        };


        public enum OptimizationLevel
        {
            None,
            Size,  
            Performance,  
        };

        public enum Environment
        {
            Vulkan,
            OpenGL,
            OpenGL_Compat,
        };

        public enum IncludeType
        {
            Relative,
            Absolute
        }

        public delegate IncludeResult IncludeHandler(string requestedSource, string requestingSource, IncludeType type);

        private IntPtr _handle;
        private InputLanguage _lang = InputLanguage.GLSL;
        private OptimizationLevel _level = OptimizationLevel.None;
        private Environment _env = Environment.Vulkan;
        private bool _debug = false;
        IncludeHandler IncludeCallback;
        ShadercNative.IncludeFunction includeFunction;
        ShadercNative.ReleaseInclude releaseInclude;
        internal IntPtr NativeHandle => _handle;

        public InputLanguage Language
        {
            get => _lang; 
            set
            {
                _lang = value;
                ShadercNative.shaderc_compile_options_set_source_language(_handle, (int)_lang);
            }
        }

        public bool GenerateDebug
        {
            get => _debug; 
            set
            {
                _debug = value;
                ShadercNative.shaderc_compile_options_set_generate_debug_info(_handle);
            }
        }

        public OptimizationLevel Optimization
        {
            get => _level; 
            set
            {
                _level = value;
                ShadercNative.shaderc_compile_options_set_optimization_level(_handle,(int)_level);
            }
        }

        public Environment Target
        {
            get => _env; 
            set
            {
                _env = value;
                ShadercNative.shaderc_compile_options_set_target_env(_handle, (int)_env, 0);
            }
        }


        public CompileOptions(IncludeHandler includeCallback = null)
        {
            IncludeCallback = includeCallback;
            _handle = ShadercNative.shaderc_compile_options_initialize();

            includeFunction = new ShadercNative.IncludeFunction(DelegateWrapper);
            releaseInclude = new ShadercNative.ReleaseInclude(ReleaseInclude);
            ShadercNative.shaderc_compile_options_set_include_callbacks(_handle, includeFunction,
                releaseInclude, IntPtr.Zero);
        }

        private CompileOptions(IntPtr handle)
        {
            _handle = handle;
        }

        ~CompileOptions()
        {
            ShadercNative.shaderc_compile_options_release(_handle);
        }


        public CompileOptions Clone()
        {
            return new CompileOptions(ShadercNative.shaderc_compile_options_clone(_handle));
        }

        private IntPtr DelegateWrapper(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string requestedSource, int type,
                                         [MarshalAs(UnmanagedType.LPStr)] string requestingSource, UIntPtr includeDepth)
        {
            IncludeResult result = null;

            if (IncludeCallback != null)
                result = IncludeCallback(requestedSource, requestingSource, (IncludeType)type);
            
            else
                result = new IncludeResult("", "Can't resolve include! No include callback specified!");
            

            IntPtr nativePtr = Marshal.AllocHGlobal(Marshal.SizeOf(result.NativeStruct));
            Marshal.StructureToPtr(result.NativeStruct, nativePtr,false);

            return nativePtr;
        }

        private void ReleaseInclude(IntPtr userData, IntPtr result)
        {
            Marshal.DestroyStructure(result,typeof(ShadercNative.IncludeResult));
        }
    }
}
