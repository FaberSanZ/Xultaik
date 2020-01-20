// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	GLFWLoader.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;



namespace Zeckoxe.Desktop.GLFWNative
{
    public static unsafe class Interop
    {
        public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr pointer) => Marshal.GetDelegateForFunctionPointer<TDelegate>(pointer);
        public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate @delegate) => Marshal.GetFunctionPointerForDelegate(@delegate);

    }

    public static class GLFWLoader
    {
        private static readonly IntPtr _handle;
        private const int LibDLRtldNow = 2;


        static GLFWLoader()
        {
            _handle = GetGLFWLibraryNameCandidates().Select(LoadLibrary).FirstOrDefault(handle => handle != IntPtr.Zero);

            if (_handle == IntPtr.Zero)
                throw new NotImplementedException("GLFW native library was not found.");
        }

        public static TDelegate GetStaticProc<TDelegate>(string procName) where TDelegate : class
        {
            IntPtr handle;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                handle = Kernel32GetProcAddress(_handle, procName);

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                handle = LibDLGetProcAddress(_handle, procName);

            else
                throw new NotImplementedException();

            return handle == IntPtr.Zero ? null : Interop.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

        private static IntPtr LoadLibrary(string fileName)
        {
            IntPtr handle;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                handle = Kernel32LoadLibrary(fileName);

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                handle = LibDLLoadLibrary(fileName, LibDLRtldNow);

            else
                throw new NotImplementedException();

            return handle;
        }

        private static IEnumerable<string> GetGLFWLibraryNameCandidates()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                yield return "glfw3.dll";

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                yield return ""; // Known to be present on Ubuntu 16.
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                yield return ""; // Using on MacOS.

            throw new NotImplementedException("Ran out of places to look for the vulkan native library");
        }


        [DllImport("kernel32", EntryPoint = "LoadLibrary")]
        private static extern IntPtr Kernel32LoadLibrary(string fileName);


        [DllImport("kernel32", EntryPoint = "GetProcAddress")]
        private static extern IntPtr Kernel32GetProcAddress(IntPtr module, string procName);


        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr LibDLLoadLibrary(string fileName, int flags);


        [DllImport("libdl", EntryPoint = "dlsym")]
        private static extern IntPtr LibDLGetProcAddress(IntPtr handle, string name);

    }
}