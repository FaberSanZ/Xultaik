using System;
using System.Runtime.InteropServices;

namespace Zeckoxe.ShaderCompiler
{
    internal class IncludeResult
    {
        internal ShadercNative.IncludeResult NativeStruct
        {
            get
            {
                return new ShadercNative.IncludeResult()
                {
                    sourceName = _sourceName,
                    sourceNameLength = new UIntPtr((uint) _sourceName.Length),
                    content = _content,
                    contentLength = new UIntPtr((uint) _content.Length),
                    userData = IntPtr.Zero
                };
            }
        }

        string _sourceName;
        string _content;

        /// <summary>
        /// Creates the result of an include directive
        /// </summary>
        /// <param name="sourceName">The resolved name of the included source</param>
        /// <param name="content">The content of the included source</param>
        internal IncludeResult(string sourceName, string content)
        {
            _sourceName = sourceName;
            _content = content;
        }

    }
}
