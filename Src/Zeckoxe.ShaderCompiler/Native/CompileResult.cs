using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.ShaderCompiler
{
    public enum Status
    {
        Success = 0,
        InvalidStage,  // error stage deduction
        CompilationError,
        InternalError,  // unexpected failure
        NullResultObject,
        InvalidAssembly,
    };

    internal class CompileResult
    {


        private readonly IntPtr _handle;

        internal IntPtr NativeHandle => _handle;

        internal ulong NumberOfWarnings => ShadercNative.shaderc_result_get_num_warnings(_handle).ToUInt64();

        internal ulong NumberOfErrors => ShadercNative.shaderc_result_get_num_errors(_handle).ToUInt64();

        internal Status CompileStatus => (Status)ShadercNative.shaderc_result_get_compilation_status(_handle);

        internal string ErrorMessage
        {
            get
            {
                IntPtr ptr = ShadercNative.shaderc_result_get_error_message(_handle);
                return Marshal.PtrToStringAnsi(ptr);
            }
        }


        internal byte[] GetBytes()
        {
            int size = (int)ShadercNative.shaderc_result_get_length(_handle);
            IntPtr nativeBuf = ShadercNative.shaderc_result_get_bytes(_handle);

            byte[] result = new byte[size];
            if (nativeBuf != IntPtr.Zero)
            {
                Marshal.Copy(nativeBuf, result, 0, size);
            }

            return result;
        }


        internal string GetString()
        {
            return Encoding.ASCII.GetString(GetBytes());
        }

        internal CompileResult(IntPtr handle)
        {
            _handle = handle;
        }
    }
}
