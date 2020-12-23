// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/




using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Graphics;
using Silk.NET;
using Silk.NET.GLFW;
using Silk.NET.Core.Loader;
using Zeckoxe.Desktop.GLFWNative;

namespace Zeckoxe.Desktop
{
    public unsafe class Window : IDisposable
    {
        private string _title;


        Glfw glfw = GlfwProvider.GLFW.Value;

        WindowHandle* pWindowHandle;

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

        public IntPtr Win32Handle => glfw.Library.LoadFunction<GLFW.glfwGetWin32Window>(nameof(GLFW.glfwGetWin32Window))(pWindowHandle);

        public string Title
        {
            get => _title;

            set
            {
                if (value != _title)
                {
                    _title = value;
                    glfw.SetWindowTitle(pWindowHandle, value);
                }
            }
        }


        public SwapchainSource GetSwapchainSource(Adapter adapter)
        {
            if (adapter.SupportsWin32Surface)
            {
                IntPtr hwnd = Win32Handle;
                IntPtr hinstance = Process.GetCurrentProcess().Handle;

                if (hwnd != IntPtr.Zero && hinstance != IntPtr.Zero)
                {
                    return SwapchainSource.CreateWin32(hwnd, hinstance);
                }
            }


            // TODO: CreateXlib
            //if ()
            //{
            //    IntPtr x11Window = GLFW.GlfwGetX11Window(pWindow);
            //    IntPtr x11Display = GLFW.GlfwGetX11Display();

            //    if (x11Display != IntPtr.Zero && x11Window != IntPtr.Zero)
            //    {
            //        return SwapchainSource.CreateXlib(x11Display, x11Window);
            //    }

            //}

            // TODO: CreateWayland
            //if () 
            //{
            //    IntPtr waylandWindow = GLFW.GlfwGetWaylandWindow(pWindow);
            //    IntPtr waylandDisplay = GLFW.GlfwGetWaylandDisplay();

            //    if (waylandWindow != IntPtr.Zero && waylandDisplay != IntPtr.Zero)
            //    {
            //        return SwapchainSource.CreateWayland(waylandDisplay, waylandWindow);
            //    }
            //}

            throw new PlatformNotSupportedException("Cannot create a SwapchainSource.");


            //ulong surface = default;


            //IntPtr hwnd = GLFW.GlfwCreateWindowSurface(adapter.GetInstance().Handle ,pWindow , null, &surface);
            //Console.WriteLine(hwnd.ToInt64());

            //return SwapchainSource.CreateWindow(surface);
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
