// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using Silk.NET.GLFW;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Xultaik.Desktop.GLFWNative
{
    internal static unsafe partial class GLFW
    {

		internal delegate IntPtr glfwGetWin32Window(WindowHandle* window);
		internal delegate IntPtr glfwGetCocoaWindow(WindowHandle* window);

		internal delegate IntPtr glfwGetX11Window(WindowHandle* window);
		internal delegate IntPtr glfwGetX11Display();

		internal delegate IntPtr glfwGetWaylandWindow(WindowHandle* window);
		internal delegate IntPtr glfwGetWaylandDisplay();

	}
}

	

