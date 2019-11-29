using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zeckoxe.ShaderCompiler
{
    public class ShaderCompiler
    {
        public enum Stage 
        {
            // Forced shader kinds. These shader kinds force the compiler to compile the
            // source code as the specified kind of shader.
        
            Vertex,
            
            Fragment,
            
            Compute,
            
            Geometry,
            
            TessControl,
            
            TessEvaluation,
        }

        private IntPtr _handle;

        internal IntPtr NativeHandle => _handle;

        public ShaderCompiler()
        {
            _handle = ShadercNative.shaderc_compiler_initialize();
        }

        ~ShaderCompiler()
        {
            ShadercNative.shaderc_compiler_release(_handle);
        }

        /// <summary>
        /// \see <see cref="Compile"/>
        /// </summary>
        public CompileResult Compile(Stream source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            using (StreamReader sr = new StreamReader(source))
            {
                return Compile(sr.ReadToEnd(), stage, options, name, entryPoint);
            }
        }

        /// <summary>
        /// Compile a given source to spirv bytecode
        /// </summary>
        /// <param name="source">The source string of the shader</param>
        /// <param name="stage">The stage in the pipeline</param>
        /// <param name="options">The compile options</param>
        /// <param name="name">The name of the shader that will be used for include directives</param>
        /// <param name="entryPoint">The name of the entry point</param>
        /// <returns>The result</returns>
        public CompileResult Compile(string source, Stage stage , CompileOptions options, string name,string entryPoint= "main")
        {       
            IntPtr resultPtr = ShadercNative.shaderc_compile_into_spv(_handle, source,new UIntPtr((uint)source.Length), (int)stage, name, entryPoint, options.NativeHandle);
            return new CompileResult(resultPtr);
        }

        /// <summary>
        /// \see <see cref="Preprocess"/>
        /// </summary>
        public CompileResult Preprocess(Stream source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            using (StreamReader sr = new StreamReader(source))
            {
                return Preprocess(sr.ReadToEnd(), stage, options, name, entryPoint);
            }
        }

        /// <summary>
        /// Similar to Compile, but instead of spv bytecode returns the preprocessed shader
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stage"></param>
        /// <param name="options"></param>
        /// <param name="name"></param>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        public CompileResult Preprocess(string source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            IntPtr resultPtr = ShadercNative.shaderc_compile_into_preprocessed_text(_handle, source, new UIntPtr((uint) source.Length), (int) stage, name, entryPoint, options.NativeHandle);
            return new CompileResult(resultPtr);
        }

        /// <summary>
        /// \see <see cref="Assemble"/>
        /// </summary>
        public CompileResult Assemble(Stream source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            using (StreamReader sr = new StreamReader(source))
            {
                return Preprocess(sr.ReadToEnd(), stage, options, name, entryPoint);
            }
        }

        /// <summary>
        /// Similar to Compile, but instead of spv bytecode returns assembled string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stage"></param>
        /// <param name="options"></param>
        /// <param name="name"></param>
        /// <param name="entryPoint"></param>
        /// <returns></returns>
        public CompileResult Assemble(string source, Stage stage, CompileOptions options, string name, string entryPoint = "main")
        {
            IntPtr resultPtr = ShadercNative.shaderc_compile_into_spv_assembly(_handle, source, new UIntPtr((uint) source.Length), (int) stage, name, entryPoint, options.NativeHandle);
            return new CompileResult(resultPtr);
        }
    }
}
