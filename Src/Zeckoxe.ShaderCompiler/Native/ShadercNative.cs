// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	ShadercNative.cs
=============================================================================*/





using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]



    internal static unsafe class ShadercNative
    {
        internal struct IncludeResult
        {
            internal string sourceName;
            internal UIntPtr sourceNameLength;

            internal string content;
            internal UIntPtr contentLength;

            internal IntPtr userData;
        }


        internal delegate IntPtr shader_ccompiler_initialize();
        private static readonly shader_ccompiler_initialize shaderc_compiler_initialize_ = Loader.GetStaticProc<shader_ccompiler_initialize>("shaderc_compiler_initialize");
        internal static IntPtr Initialize() => shaderc_compiler_initialize_();






        internal delegate IntPtr shaderc_compiler_release(IntPtr compiler);
        private static readonly shaderc_compiler_release shaderc_compiler_release_ = Loader.GetStaticProc<shaderc_compiler_release>("shaderc_compiler_release");
        internal static IntPtr Release(IntPtr compiler) => shaderc_compiler_release_(compiler);





        internal delegate IntPtr shaderc_compile_into_spv(IntPtr compiler, byte* source, UIntPtr sourceSize, int kind, byte* filename, byte* entryPoint, IntPtr options);
        private static readonly shaderc_compile_into_spv shaderc_compile_into_spv_ = Loader.GetStaticProc<shaderc_compile_into_spv>("shaderc_compile_into_spv");
        internal static IntPtr CompileIntoSPV(IntPtr compiler, byte* source, UIntPtr sourceSize, int kind, byte* filename, byte* entryPoint, IntPtr options) 
            => shaderc_compile_into_spv_( compiler,  source,  sourceSize,  kind, filename,  entryPoint,  options);





        internal delegate IntPtr shaderc_compile_into_preprocessed_text(IntPtr compiler, byte* source, UIntPtr sourceSize, int kind, byte* filename, byte* entryPoint,IntPtr options);
        private static readonly shaderc_compile_into_preprocessed_text shaderc_compile_into_preprocessed_text_ = Loader.GetStaticProc<shaderc_compile_into_preprocessed_text>("shaderc_compile_into_preprocessed_text");
        internal static IntPtr CompileIntoPreprocessedText(IntPtr compiler, byte* source, UIntPtr sourceSize, int kind, byte* filename, byte* entryPoint, IntPtr options) 
            => shaderc_compile_into_preprocessed_text_( compiler, source,  sourceSize,  kind,   filename,   entryPoint,  options);




        internal delegate IntPtr shaderc_compile_options_initialize();
        private static readonly shaderc_compile_options_initialize shaderc_compile_options_initialize_ = Loader.GetStaticProc<shaderc_compile_options_initialize>("shaderc_compile_options_initialize");
        internal static IntPtr CompileOptionsInitialize() => shaderc_compile_options_initialize_();



        internal delegate IntPtr shaderc_compile_options_release(IntPtr options);
        private static readonly shaderc_compile_options_release shaderc_compile_options_release_ = Loader.GetStaticProc<shaderc_compile_options_release>("shaderc_compile_options_release");
        internal static IntPtr CompileOptionsRelease(IntPtr options) => shaderc_compile_options_release_(options);






        internal delegate IntPtr shaderc_compile_options_set_source_language(IntPtr options, int lang);
        private static readonly shaderc_compile_options_set_source_language shaderc_compile_options_set_source_language_ = Loader.GetStaticProc<shaderc_compile_options_set_source_language>("shaderc_compile_options_set_source_language");
        internal static IntPtr CompileOptionsSetSourceLanguage(IntPtr options, int lang) => shaderc_compile_options_set_source_language_(options, lang);






        internal delegate void shaderc_compile_options_set_generate_debug_info(IntPtr options);
        private static readonly shaderc_compile_options_set_generate_debug_info shaderc_compile_options_set_generate_debug_info_ = Loader.GetStaticProc<shaderc_compile_options_set_generate_debug_info>("shaderc_compile_options_set_generate_debug_info");
        internal static void CompileOptionsSetGenerateDebugInfo(IntPtr options) => shaderc_compile_options_set_generate_debug_info_(options);




        internal delegate void shaderc_compile_options_set_optimization_level(IntPtr options, int level);
        private static readonly shaderc_compile_options_set_optimization_level shaderc_compile_options_set_optimization_level_ = Loader.GetStaticProc<shaderc_compile_options_set_optimization_level>("shaderc_compile_options_set_optimization_level");
        internal static void CompileOptionsSetOptimizationLevel(IntPtr options, int level) => shaderc_compile_options_set_optimization_level_(options, level);




        internal delegate void shaderc_compile_options_set_target_env(IntPtr options, int level, uint version);
        private static readonly shaderc_compile_options_set_target_env shaderc_compile_options_set_target_env_ = Loader.GetStaticProc<shaderc_compile_options_set_target_env>("shaderc_compile_options_set_target_env");
        internal static void CompileOptionsSetTarget(IntPtr options, int level, uint version) => shaderc_compile_options_set_target_env_(options, level, version);




        //--RESULT FUNCTIONS--

        internal delegate void shaderc_result_release(IntPtr result);
        private static readonly shaderc_result_release shaderc_result_release_ = Loader.GetStaticProc<shaderc_result_release>("shaderc_result_release");
        internal static void ResultRelease(IntPtr result) => shaderc_result_release_(result);



        internal delegate int shaderc_result_get_length(IntPtr result);
        private static readonly shaderc_result_get_length shaderc_result_get_length_ = Loader.GetStaticProc<shaderc_result_get_length>("shaderc_result_get_length");
        internal static int ResultGetLength(IntPtr result) => shaderc_result_get_length_(result);




        internal delegate UIntPtr shaderc_result_get_num_warnings(IntPtr result);
        private static readonly shaderc_result_get_num_warnings shaderc_result_get_num_warnings_ = Loader.GetStaticProc<shaderc_result_get_num_warnings>("shaderc_result_get_num_warnings");
        internal static UIntPtr ResultGetNumWarnings(IntPtr result) => shaderc_result_get_num_warnings_(result);




        internal delegate UIntPtr shaderc_result_get_num_errors(IntPtr result);
        private static readonly shaderc_result_get_num_errors shaderc_result_get_num_errors_ = Loader.GetStaticProc<shaderc_result_get_num_errors>("shaderc_result_get_num_errors");
        internal static UIntPtr ResultGetNumErrors(IntPtr result) => shaderc_result_get_num_errors_(result);





        internal delegate int shaderc_result_get_compilation_status(IntPtr result);
        private static readonly shaderc_result_get_compilation_status shaderc_result_get_compilation_status_ = Loader.GetStaticProc<shaderc_result_get_compilation_status>("shaderc_result_get_compilation_status");
        internal static int ResultGetCompilationStatus(IntPtr result) => shaderc_result_get_compilation_status_(result);




        internal delegate IntPtr shaderc_result_get_bytes(IntPtr result);
        private static readonly shaderc_result_get_bytes shaderc_result_get_bytes_ = Loader.GetStaticProc<shaderc_result_get_bytes>("shaderc_result_get_bytes");
        internal static IntPtr ResultGetBytes(IntPtr result) => shaderc_result_get_bytes_(result);




        internal delegate IntPtr shaderc_result_get_error_message(IntPtr result);
        private static readonly shaderc_result_get_error_message shaderc_result_get_error_message_ = Loader.GetStaticProc<shaderc_result_get_error_message>("shaderc_result_get_error_message");
        internal static IntPtr ResultGetErrorMessage(IntPtr result) => shaderc_result_get_error_message_(result);


    }
}
