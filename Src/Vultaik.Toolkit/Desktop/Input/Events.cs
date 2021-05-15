// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Events.cs
=============================================================================*/

using System;
using Silk.NET.GLFW;

namespace Vultaik.Desktop
{
    public class Events : IDisposable
    {
        public unsafe Events(WindowHandle* handle)
        {
            Handle = handle;
            GlfwProvider.GLFW.Value.SetCharCallback(handle, (a, b) => Char?.Invoke(a, b));
            GlfwProvider.GLFW.Value.SetKeyCallback(handle, (a, b, c, d, e) => Key?.Invoke(a, b, c, d, e));
            GlfwProvider.GLFW.Value.SetMouseButtonCallback(handle, (a, b, c, d) => MouseButton?.Invoke(a, b, c, d));
            GlfwProvider.GLFW.Value.SetCursorEnterCallback(handle, (a, b) => CursorEnter?.Invoke(a, b));
            GlfwProvider.GLFW.Value.SetCursorPosCallback(handle, (a, b, c) => CursorPos?.Invoke(a, b, c));
            GlfwProvider.GLFW.Value.SetScrollCallback(handle, (a, b, c) => Scroll?.Invoke(a, b, c));
        }

        public unsafe WindowHandle* Handle { get; }
        public event GlfwCallbacks.KeyCallback Key;
        public event GlfwCallbacks.CharCallback Char;
        public event GlfwCallbacks.MouseButtonCallback MouseButton;
        public event GlfwCallbacks.CursorEnterCallback CursorEnter;
        public event GlfwCallbacks.CursorPosCallback CursorPos;
        public event GlfwCallbacks.ScrollCallback Scroll;

        public unsafe void Dispose()
        {
            GlfwProvider.GLFW.Value.SetCharCallback(Handle, null);
            GlfwProvider.GLFW.Value.SetKeyCallback(Handle, null);
            GlfwProvider.GLFW.Value.SetMouseButtonCallback(Handle, null);
            GlfwProvider.GLFW.Value.SetCursorEnterCallback(Handle, null);
            GlfwProvider.GLFW.Value.SetCursorPosCallback(Handle, null);
            GlfwProvider.GLFW.Value.SetScrollCallback(Handle, null);
        }
    }
}
