using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    public class CompileResult
    {
        /// <summary>
        /// Gives information about the result of a compilation
        /// </summary>
        public enum Status
        {
            Success = 0,
            InvalidStage,  // error stage deduction
            CompilationError,
            InternalError,  // unexpected failure
            NullResultObject,
            InvalidAssembly,
        };

        IntPtr _handle;

        internal IntPtr NativeHandle => _handle;
        /// <summary>
        /// Get the number of warnings
        /// </summary>
        public ulong NumberOfWarnings
        {
            get
            {
                return ShadercNative.shaderc_result_get_num_warnings(_handle).ToUInt64();
            }
        }
        /// <summary>
        /// Get the number if errors
        /// </summary>
        public ulong NumberOfErrors
        {
            get
            {
                return ShadercNative.shaderc_result_get_num_errors(_handle).ToUInt64();
            }
        }
        /// <summary>
        /// Get the compile status
        /// </summary>
        public Status CompileStatus
        {
            get
            {
                return (Status)ShadercNative.shaderc_result_get_compilation_status(_handle);
            }
        }
        /// <summary>
        /// Get the error message if there is one
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                IntPtr ptr = ShadercNative.shaderc_result_get_error_message(_handle);
                return Marshal.PtrToStringAnsi(ptr);
            }
        }

        /// <summary>
        /// Get the produced SpirV bytecode
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            int size = (int)ShadercNative.shaderc_result_get_length(_handle);
            IntPtr nativeBuf = ShadercNative.shaderc_result_get_bytes(_handle);
            byte[] result = new byte[size];
            Marshal.Copy(nativeBuf, result, 0, size);

            return result;
        }

        /// <summary>
        /// Get the produced Assembly/ Preprocessed shader
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
           return Encoding.ASCII.GetString(GetBytes());
        }

        internal CompileResult(IntPtr handle)
        {
            _handle = handle;
        }
    }
}
