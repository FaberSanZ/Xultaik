using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    public class CompileOptions
    {
        /// <summary>
        /// The type of language that should be processed
        /// </summary>
        public enum InputLanguage
        {
            GLSL = 0,
            HLSL = 1,
        };

        /// <summary>
        /// The level of optimization that should be applied
        /// </summary>
        public enum OptimizationLevel
        {
            None,
            Size,  // optimize towards reducing code size
            Performance,  // optimize towards performance
        };

        /// <summary>
        /// The target that we are aiming for
        /// </summary>
        public enum Environment
        {
            Vulkan,
            OpenGL,
            OpenGL_Compat,
        };

        /// <summary>
        /// Which kind of include method was chosen
        /// </summary>
        public enum IncludeType
        {
            Relative,
            Absolute
        }

        /// <summary>
        /// The callback that will be called when an include happens
        /// </summary>
        /// <param name="requestedSource">The name of the source that should be included</param>
        /// <param name="requestingSource">The name of the source where the include directive is performed</param>
        /// <param name="type">Specifies how the file should be searched for. #include <file> or #include "file"</file></param>
        /// <returns>The result of the include statement</returns>
        public delegate IncludeResult IncludeHandler(string requestedSource, string requestingSource, IncludeType type);

        private IntPtr _handle;
        private InputLanguage _lang = InputLanguage.GLSL;
        private OptimizationLevel _level = OptimizationLevel.None;
        private Environment _env = Environment.Vulkan;
        private bool _debug = false;

        internal IntPtr NativeHandle => _handle;

        /// <summary>
        /// Specifies the language <see cref="InputLanguage"/>
        /// </summary>
        public InputLanguage Language
        {
            get { return _lang; }
            set
            {
                _lang = value;
                ShadercNative.shaderc_compile_options_set_source_language(_handle, (int)_lang);
            }
        }
        /// <summary>
        /// Wether or not debug information should be generated
        /// </summary>
        public bool GenerateDebug
        {
            get { return _debug; }
            set
            {
                _debug = value;
                ShadercNative.shaderc_compile_options_set_generate_debug_info(_handle);
            }
        }
        /// <summary>
        /// The kind of optimization that should be applied to the spirv bytecode
        /// </summary>
        public OptimizationLevel Optimization
        {
            get { return _level; }
            set
            {
                _level = value;
                ShadercNative.shaderc_compile_options_set_optimization_level(_handle,(int)_level);
            }
        }
        /// <summary>
        /// The Graphics API that we are aiming for
        /// </summary>
        public Environment Target
        {
            get { return _env; }
            set
            {
                _env = value;
                //TODO add proper version support
                ShadercNative.shaderc_compile_options_set_target_env(_handle, (int)_env, 0);
            }
        }
        /// <summary>
        /// Specify the call back <see cref="IncludeHandler"/>
        /// </summary>
        IncludeHandler IncludeCallback;

        ShadercNative.IncludeFunction includeFunction;
        ShadercNative.ReleaseInclude releaseInclude;
        /// <summary>
        /// Create new compile options
        /// </summary>
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

        /// <summary>
        /// Make a clone
        /// </summary>
        public CompileOptions Clone()
        {
            return new CompileOptions(ShadercNative.shaderc_compile_options_clone(_handle));
        }

        private IntPtr DelegateWrapper(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string requestedSource, int type,
                                         [MarshalAs(UnmanagedType.LPStr)] string requestingSource, UIntPtr includeDepth)
        {
            IncludeResult result = null;

            if (IncludeCallback != null)
            {
                result = IncludeCallback(requestedSource, requestingSource, (IncludeType)type);
            }
            else
            {
                result = new IncludeResult("", "Can't resolve include! No include callback specified!");
            }

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
