// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Desktop.GLFWNative;

namespace Zeckoxe.Desktop
{
    public unsafe class Window : IDisposable
    {
        private string _title;


        public string Title
        {
            get => _title; 

            set
            {
                if (value != _title)
                {
                    _title = value;
                    GLFW.GlfwSetWindowTitle(pWindow, Zeckoxe.Core.Interop.String.ToPointer(value));
                }
            }
        }





        public int Width { get; set; }
        public int Height { get; set; }
        public IntPtr Win32Handle => GLFW.GlfwGetWin32Window(pWindow);




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
