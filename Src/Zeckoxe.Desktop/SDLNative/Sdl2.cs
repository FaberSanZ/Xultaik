using NativeLibraryLoader;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NativeLibrary = NativeLibraryLoader.NativeLibrary;

namespace Zeckoxe.Sdl2
{
    public static unsafe partial class Sdl2Native
    {
        private static readonly NativeLibrary s_sdl2Lib = LoadSdl2();
        private static NativeLibrary LoadSdl2()
        {
            string[] names;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                names = new[] { "SDL2.dll" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                names = new[]
                {
                    "libSDL2-2.0.so",
                    "libSDL2-2.0.so.0",
                    "libSDL2-2.0.so.1",
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                names = new[]
                {
                    "libsdl2.dylib"
                };
            }
            else
            {
                Debug.WriteLine("Unknown SDL platform. Attempting to load \"SDL2\"");
                names = new[] { "SDL2.dll" };
            }

            NativeLibrary lib = new NativeLibrary(names);
            return lib;
        }

        /// <summary>
        /// Loads an SDL2 function by the given name.
        /// </summary>
        /// <typeparam name="T">The delegate type of the function to load.</typeparam>
        /// <param name="name">The name of the exported native function.</param>
        /// <returns>A delegate which can be used to invoke the native function.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when no function with the given name is exported by SDL2.
        /// </exception>
        public static T LoadFunction<T>(string name)
        {
            try
            {
                return s_sdl2Lib.LoadFunction<T>(name);
            }
            catch
            {
                Debug.WriteLine(
                    $"Unable to load SDL2 function \"{name}\". " +
                    $"Attempting to call this function will cause an exception to be thrown.");
                return default(T);
            }
        }

    }
}
