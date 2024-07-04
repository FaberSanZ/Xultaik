// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Xultaik.Desktop
{
    [Flags]
    public enum NativeWindow : ulong
    {
        Glfw = 1,
        Sdl = 2,
        Win32 = 512,
        X11 = 1024,
        DirectFB = 2048,
        Cocoa = 4096,
        UIKit = 8192,
        Wayland = 16384,
        WinRT = 32768,
        Android = 65536,
        Vivante = 131072,
        OS2 = 262144,
        Haiku = 524288
    }



}
