//// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/



using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zeckoxe.Desktop.Win32;
using static Zeckoxe.Desktop.Win32.Win32Native;

namespace Zeckoxe.Desktop
{
    public unsafe class Window : IDisposable
    {

        public IntPtr HWND { get; private set; }

        public IntPtr HInstance => GetModuleHandle(string.Empty);

        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public IntPtr Handle { get; private set; }



        // internal
        int x;
        int y;
        internal bool paused;
        internal bool exitRequested;
        internal WNDPROC wndProc;
        internal BorderStyle BorderStyle;
        internal WindowStyles style;
        internal const int CW_USEDEFAULT = unchecked((int)0x80000000);


        public Window(string title, int width, int height, BorderStyle borderStyle)
        {    
            Title = title;
            Width = width;
            Height = height;
            BorderStyle = borderStyle;
            wndProc = ProcessWindowMessage;

            Recreate();
        }

        private void Recreate()
        {
            switch (BorderStyle)
            {
                case BorderStyle.None:
                    style = WindowStyles.Visible | WindowStyles.Popup | WindowStyles.ClipChiLdren;
                    break;

                case BorderStyle.SizableToolWindow:
                    style = WindowStyles.Popup | WindowStyles.Border | WindowStyles.Caption | WindowStyles.SysMenu;
                    break;

                case BorderStyle.Sizable:
                    style = WindowStyles.OverlappedWindow;
                    break;
                default:
                    break;
            }
            //Console.WriteLine(Unsafe.SizeOf<WNDCLASSEX>());
            var wndClassEx = new WNDCLASSEX
            {
                Size =  48,
                Styles = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW | WindowClassStyles.CS_OWNDC,
                WindowProc = wndProc,
                InstanceHandle = HInstance,
                CursorHandle = LoadCursor(IntPtr.Zero, SystemCursor.IDC_ARROW),
                BackgroundBrushHandle = IntPtr.Zero,
                IconHandle = IntPtr.Zero,
                ClassName = "Windows",
            };

            int atom = RegisterClassEx(ref wndClassEx);

            //if (atom == 0)
            //    throw new InvalidOperationException($"Failed to register window class. Error: {Marshal.GetLastWin32Error()}");


            //else
            int screenWidth = GetSystemMetrics(SystemMetrics.SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SystemMetrics.SM_CYSCREEN);

            if (Width > 0 && Height > 0)
            {


                // Place the window in the middle of the screen.WS_EX_APPWINDOW
                x = (screenWidth - Width) / 2;
                y = (screenHeight - Height) / 2;
            }



            if (Width > 0 && Height > 0)
            {
                Rect rect = new Rect(0, 0, Width, Height);

                // Adjust according to window styles
                AdjustWindowRectEx(&rect, style, false, 0);

                Width = rect.Right - rect.Left;
                Height = rect.Bottom - rect.Top;
            }
            else
            {
                x = y = Width = Height = CW_USEDEFAULT;
            }

            IntPtr PtrZero = IntPtr.Zero;

            HWND = CreateWindowEx(0, "Windows", Title, style, x, y, Width, Height, PtrZero, PtrZero, PtrZero, PtrZero);

            Handle = HWND;


        }


        public void Show()
        {

            ShowWindow(HWND, ShowWindowCommand.Normal);
        }


        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                IntPtr destroyHandle = Handle;
                Handle = IntPtr.Zero;
                DestroyWindow(destroyHandle);
            }
        }





        public IntPtr ProcessWindowMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)WindowMessage.ActivateApp)
            {
                paused = (int)wParam.ToInt64() == 0;


                return DefWindowProc(hWnd, msg, wParam, lParam);
            }

            switch ((WindowMessage)msg)
            {
                case WindowMessage.Destroy:
                    PostQuitMessage(0);
                    break;
            }

            return DefWindowProc(hWnd, msg, wParam, lParam);
        }




        public void RenderLoop(Action Draw)
        {

            while (!exitRequested)
            {
                if (!paused)
                {
                    const uint PM_REMOVE = 1;
                    if (PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                    {
                        TranslateMessage(ref msg);
                        DispatchMessage(ref msg);

                        if (msg.Value == (uint)WindowMessage.Quit)
                        {
                            exitRequested = true;
                            break;
                        }
                    }

                    Draw();
                }
                else
                {
                    int ret = GetMessage(out var msg, IntPtr.Zero, 0, 0);
                    if (ret == 0)
                    {
                        exitRequested = true;
                        break;
                    }
                    else if (ret == -1)
                    {
                        exitRequested = true;
                        break;
                    }
                    else
                    {
                        TranslateMessage(ref msg);
                        DispatchMessage(ref msg);
                    }
                }
            }
        }


    }
}
