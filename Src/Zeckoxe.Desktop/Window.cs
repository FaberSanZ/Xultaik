// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/




using Silk.NET.GLFW;
using System;
using System.Diagnostics;
using Zeckoxe.Desktop.GLFWNative;
using Zeckoxe.Core;

namespace Zeckoxe.Desktop
{
    public unsafe class Window : IDisposable
    {
        private string _title;
        private readonly Glfw glfw = GlfwProvider.GLFW.Value;
        private readonly WindowHandle* pWindowHandle;

        internal IntPtr WindowHandle => new IntPtr(pWindowHandle);

        public Window(string title, int width, int height)
        {
            _title = title;
            Width = width;
            Height = height;

            glfw.WindowHint(WindowHintBool.Visible, false);

            pWindowHandle = glfw.CreateWindow(width, height, _title, (Monitor*)IntPtr.Zero.ToPointer(), null);
        }


        public int Width { get; set; }
        public int Height { get; set; }



        public (int Width, int Height) FramebufferSize
        {
            get
            {
                glfw.GetFramebufferSize(pWindowHandle, out int width, out int height);

                return (width, height);
            }
        }

        public string Title
        {
            get => _title;
            set => glfw.SetWindowTitle(pWindowHandle, _title = value);
        }


        public IntPtr Win32Handle => glfw.Library.LoadFunction<GLFW.glfwGetWin32Window>(nameof(GLFW.glfwGetWin32Window))(pWindowHandle);

        public IntPtr CocoaWindowHandle => glfw.Library.LoadFunction<GLFW.glfwGetCocoaWindow>(nameof(GLFW.glfwGetCocoaWindow))(pWindowHandle);

        public IntPtr X11WindowHandle => glfw.Library.LoadFunction<GLFW.glfwGetX11Window>(nameof(GLFW.glfwGetX11Window))(pWindowHandle);
        public IntPtr X11DisplayHandle => glfw.Library.LoadFunction<GLFW.glfwGetX11Display>(nameof(GLFW.glfwGetX11Display))();

        public IntPtr WaylandWindowHandle => glfw.Library.LoadFunction<GLFW.glfwGetWaylandWindow>(nameof(GLFW.glfwGetX11Window))(pWindowHandle);
        public IntPtr WaylandDisplayHandle => glfw.Library.LoadFunction<GLFW.glfwGetWaylandDisplay>(nameof(GLFW.glfwGetX11Display))();


        public SwapchainSource SwapchainWin32
        {
            get
            {
                IntPtr hwnd = Win32Handle;
                IntPtr hinstance = Process.GetCurrentProcess().Handle;

                if (hwnd == IntPtr.Zero && hinstance == IntPtr.Zero)
                    return SwapchainSource.CreateWin32(IntPtr.Zero, IntPtr.Zero);

                return SwapchainSource.CreateWin32(hwnd, hinstance);
            }
        }


        public SwapchainSource SwapchainX11
        {
            get
            {
                IntPtr Display = X11DisplayHandle;
                IntPtr Window = X11WindowHandle;

                if (Window == IntPtr.Zero && Display == IntPtr.Zero)
                    return SwapchainSource.CreateWin32(IntPtr.Zero, IntPtr.Zero);

                return SwapchainSource.CreateXlib(Display, Window);
            }
        }


        public SwapchainSource SwapchainWayland
        {
            get
            {
                IntPtr Display = WaylandDisplayHandle;
                IntPtr Window = WaylandWindowHandle;

                if (Window == IntPtr.Zero && Display == IntPtr.Zero)
                    return SwapchainSource.CreateWin32(IntPtr.Zero, IntPtr.Zero);

                return SwapchainSource.CreateXlib(Display, Window);
            }
        }


        public SwapchainSource SwapchainNS
        {
            get
            {
                IntPtr Window = CocoaWindowHandle;

                if (Window == IntPtr.Zero)
                    return SwapchainSource.CreateNSWindow(IntPtr.Zero);

                return SwapchainSource.CreateNSWindow(Window);
            }
        }


        public void RenderLoop(Action render)
        {

            while (!glfw.WindowShouldClose(pWindowHandle))
            {
                render();

                glfw.PollEvents();
            }

        }

        public void Show()
        {
            glfw.ShowWindow(pWindowHandle);
        }

        public void Dispose()
        {

        }
    }



}
