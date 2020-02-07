using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    internal static unsafe class ShadercNative
    {

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_compiler_initialize();

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compiler_release(IntPtr compiler);

        [DllImport("shaderc_shared", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_compile_into_spv(IntPtr compiler, byte* source, UIntPtr sourceSize, int kind, byte* filename, byte* entryPoint, IntPtr options);



        [DllImport("shaderc_shared", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_compile_into_preprocessed_text(IntPtr compiler, [MarshalAs(UnmanagedType.LPStr)] string source,
        UIntPtr sourceSize, int kind, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string entryPoint,
        IntPtr options);

        //OPTIONS FUNCTIONS
        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_compile_options_initialize();

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_compile_options_clone(IntPtr options);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_release(IntPtr options);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_set_source_language(IntPtr options, int lang);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_set_generate_debug_info(IntPtr options);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_set_optimization_level(IntPtr options, int level);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_set_target_env(IntPtr options, int level, uint version);

        //RESULT FUNCTIONS
        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_result_release(IntPtr result);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr shaderc_result_get_length(IntPtr result);

        [DllImport("shaderc_shared")]
        internal static extern UIntPtr shaderc_result_get_num_warnings(IntPtr result);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr shaderc_result_get_num_errors(IntPtr result);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int shaderc_result_get_compilation_status(IntPtr result);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_result_get_bytes(IntPtr result);

        [DllImport("shaderc_shared", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr shaderc_result_get_error_message(IntPtr result);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct IncludeResult
        {
            internal string sourceName;
            internal UIntPtr sourceNameLength;

            internal string content;
            internal UIntPtr contentLength;

            internal IntPtr userData;
        }

        internal delegate IntPtr IncludeFunction(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string requestedSource, int type,
                                         [MarshalAs(UnmanagedType.LPStr)] string requestingSource, UIntPtr includeDepth);

        internal delegate void ReleaseInclude(IntPtr userData, IntPtr result);

        [DllImport("shaderc_shared", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void shaderc_compile_options_set_include_callbacks(IntPtr options, [MarshalAs(UnmanagedType.FunctionPtr)] IncludeFunction resolver,
                                        [MarshalAs(UnmanagedType.FunctionPtr)] ReleaseInclude resultReleaser, IntPtr userData);
    }
}
