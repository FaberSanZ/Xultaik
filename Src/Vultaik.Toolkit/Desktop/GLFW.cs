// Copyright(c) 2019-2021 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	GLFW.cs
=============================================================================*/


using Silk.NET.GLFW;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Vultaik.Desktop.GLFWNative
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

	

