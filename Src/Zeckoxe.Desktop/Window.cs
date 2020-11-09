// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/



using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.Desktop.GLFWNative;
using Zeckoxe.Graphics;

namespace Zeckoxe.Desktop
{
    public unsafe class Window : IDisposable
    {
        private string _title;






        internal IntPtr pWindow { get; private set; }

        public Window(string title, int width, int height)
        {
            _title = title;
            Width = width;
            Height = height;

            GLFW.GlfwInit();
            GLFW.GlfwInitHint(GLFW.GLFW_VISIBLE, 0);

            pWindow = GLFW.GlfwCreateWindow(width, height, _title, IntPtr.Zero, IntPtr.Zero);
        }


        public int Width { get; set; }
        public int Height { get; set; }
        public IntPtr Win32Handle => GLFW.GlfwGetWin32Window(pWindow);

        public string Title
        {
            get => _title;

            set
            {
                if (value != _title)
                {
                    _title = value;
                    GLFW.GlfwSetWindowTitle(pWindow, Interop.String.ToPointer(value));
                }
            }
        }









        public SwapchainSource GetSwapchainSource(Adapter adapter)
        {
            if (adapter.SupportsWin32Surface)
            {
                IntPtr hwnd = GLFW.GlfwGetWin32Window(pWindow);
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

            while (GLFW.GlfwWindowShouldClose(pWindow) == 0)
            {
                render();

                GLFW.GlfwPollEvents();
            }

        }

        public void Show()
        {
            GLFW.ShowWindow(pWindow);
        }

        public void Dispose()
        {

        }
    }



}
