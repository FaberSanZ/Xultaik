// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Silk.NET.GLFW;
using System;
using System.Diagnostics;
using Vultaik.Desktop.GLFWNative;
using System.Runtime.InteropServices;

namespace Vultaik.Desktop
{

    public enum WindowState
    {
        /// <summary>
        /// The window is in its regular configuration.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The window has been minimized to the task bar.
        /// </summary>
        Minimized,

        /// <summary>
        /// The window has been maximized, covering the entire desktop, but not the taskbar.
        /// </summary>
        Maximized,

        /// <summary>
        /// The window has been fullscreened, covering the entire surface of the monitor.
        /// </summary>
        Fullscreen
    }

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

    public unsafe class Window : IDisposable
    {
        private string _title;
        private readonly Glfw glfw = GlfwProvider.GLFW.Value;
        internal readonly WindowHandle* pWindowHandle;

        internal IntPtr WindowHandle => new IntPtr(pWindowHandle);
        internal GlfwNativeWindow glfw_native => new(glfw, pWindowHandle);



        private GlfwCallbacks.WindowPosCallback? _onMove;
        private GlfwCallbacks.WindowSizeCallback? _onResize;
        private GlfwCallbacks.FramebufferSizeCallback? _onFramebufferResize;
        private GlfwCallbacks.DropCallback? _onFileDrop;
        private GlfwCallbacks.WindowCloseCallback? _onClosing;
        private GlfwCallbacks.WindowFocusCallback? _onFocusChanged;
        private GlfwCallbacks.WindowIconifyCallback? _onMinimized;
        private GlfwCallbacks.WindowMaximizeCallback? _onMaximized;

        private bool is_render;

        public Window(string title, int width, int height)
        {
            _title = title;
            Width = width;
            Height = height;

            glfw.Init();
            glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
            glfw.WindowHint(WindowHintBool.Visible, false);

            pWindowHandle = glfw.CreateWindow(width, height, _title, (Monitor*)IntPtr.Zero.ToPointer(), null);

            Load?.Invoke();

            _onMove = (window, x, y) =>
            {
                Move?.Invoke((x, y));
            };

            _onResize = (window, width, height) =>
            {
                if (is_render)
                    Resize?.Invoke((width, height));
            };

            _onFramebufferResize = (window, width, height) =>
            {
                FramebufferResize?.Invoke((width, height));
            };

            _onClosing = window => Closing?.Invoke();

            _onFocusChanged = (window, isFocused) => FocusChanged?.Invoke(isFocused);

            _onMinimized = (window, isMinimized) =>
            {
                WindowState state;
                // If minimized, we immediately know what value the new WindowState is.
                if (isMinimized)
                {
                    state = WindowState.Minimized;
                }
                else
                {
                    // Otherwise, we have to query a few things to figure out out.
                    if (glfw.GetWindowAttrib(pWindowHandle, WindowAttributeGetter.Maximized))
                    {
                        state = WindowState.Maximized;
                    }
                    else if (glfw.GetWindowMonitor(pWindowHandle) != null)
                    {
                        state = WindowState.Fullscreen;
                    }
                    else
                    {
                        state = WindowState.Normal;
                    }
                }

                StateChanged?.Invoke(state);
            };

            _onMaximized = (window, isMaximized) =>
            {
                // Same here as in onMinimized.
                WindowState state;
                if (isMaximized)
                {
                    state = WindowState.Maximized;
                }
                else
                {
                    if (glfw.GetWindowAttrib(pWindowHandle, WindowAttributeGetter.Iconified))
                    {
                        state = WindowState.Minimized;
                    }
                    else if (glfw.GetWindowMonitor(pWindowHandle) != null)
                    {
                        state = WindowState.Fullscreen;
                    }
                    else
                    {
                        state = WindowState.Normal;
                    }
                }

                StateChanged?.Invoke(state);
            };

            _onFileDrop = (window, count, paths) =>
            {
                var arrayOfPaths = new string[count];

                if (count == 0 || paths == IntPtr.Zero)
                {
                    return;
                }

                for (var i = 0; i < count; i++)
                {
                    var p = Marshal.ReadIntPtr(paths, i * IntPtr.Size);
                    arrayOfPaths[i] = Marshal.PtrToStringAnsi(p);
                }

                FileDrop?.Invoke(arrayOfPaths);
            };


            glfw.SetWindowPosCallback(pWindowHandle, _onMove);
            glfw.SetWindowSizeCallback(pWindowHandle, _onResize);
            glfw.SetWindowCloseCallback(pWindowHandle, _onClosing);
            glfw.SetWindowFocusCallback(pWindowHandle, _onFocusChanged);
            glfw.SetWindowIconifyCallback(pWindowHandle, _onMinimized);
            glfw.SetWindowMaximizeCallback(pWindowHandle, _onMaximized);
            glfw.SetFramebufferSizeCallback(pWindowHandle, _onFramebufferResize);
            glfw.SetDropCallback(pWindowHandle, _onFileDrop);


            
        }
        public NativeWindow NativeWindow => (NativeWindow)glfw_native.Kind;

        public event Action<(int X, int Y)>? Move;
        public event Action<WindowState>? StateChanged;
        public event Action<string[]>? FileDrop;
        public event Action<(int Width, int Height)>? Resize;
        public event Action<(int Width, int Height)>? FramebufferResize;
        public event Action? Closing;
        public event Action<bool>? FocusChanged;
        public event Action? Load;
        public event Action<double>? Update;


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

        public IntPtr Win32Handle => glfw_native.Win32!.Value.Hwnd;

        public IntPtr CocoaWindowHandle => glfw_native.Cocoa.Value;

        public IntPtr X11WindowHandle => (IntPtr)(uint)glfw_native.X11.Value.Window;
        public IntPtr X11DisplayHandle => glfw_native.X11.Value.Display;

        public IntPtr WaylandWindowHandle => glfw_native.Wayland.Value.Surface;
        public IntPtr WaylandDisplayHandle => glfw_native.Wayland.Value.Display;


        public SwapchainSource? SwapchainWin32
        {
            get
            {
                IntPtr hwnd = Win32Handle;
                IntPtr hinstance = Process.GetCurrentProcess().Handle;

                if (hwnd == IntPtr.Zero && hinstance == IntPtr.Zero)
                    return null;

                return SwapchainSource.CreateWin32(hwnd, hinstance);
            }
        }


        public SwapchainSource? SwapchainX11
        {
            get
            {
                IntPtr Display = X11DisplayHandle;
                IntPtr Window = X11WindowHandle;

                if (Window == IntPtr.Zero && Display == IntPtr.Zero)
                    return null;

                return SwapchainSource.CreateXlib(Display, Window);
            }
        }


        public SwapchainSource? SwapchainWayland
        {
            get
            {
                IntPtr Display = WaylandDisplayHandle;
                IntPtr Window = WaylandWindowHandle;

                if (Window == IntPtr.Zero && Display == IntPtr.Zero)
                    return null;

                return SwapchainSource.CreateXlib(Display, Window);
            }
        }


        public SwapchainSource? SwapchainNS
        {
            get
            {
                IntPtr Window = CocoaWindowHandle;

                if (Window == IntPtr.Zero)
                    return null;

                return SwapchainSource.CreateNSWindow(Window);
            }
        }



        public Input Input => new Input(this);



        public void RenderLoop(Action render)
        {
            bool isClosing = false;



            //while (!isClosing)
            //{
            //    if (Input.Keyboards[0].IsKeyPressed(Key.Escape))
            //        isClosing = true;

            //    Console.WriteLine(isClosing);


            //}



            while (!glfw.WindowShouldClose(pWindowHandle) && !Input.Keyboards[0].IsKeyPressed(Key.Escape))
            {
                is_render = true;
                render();
                glfw.PollEvents();
            }

        }


        public void Reset()
        {

            //CoreReset();
            UnregisterCallbacks();
        }

        private void UnregisterCallbacks()
        {
            if (_onClosing is not null)
            {
                glfw.GcUtility.Unpin(_onClosing);
                glfw.GcUtility.Unpin(_onMaximized);
                glfw.GcUtility.Unpin(_onMinimized);
                glfw.GcUtility.Unpin(_onMove);
                glfw.GcUtility.Unpin(_onResize);
                glfw.GcUtility.Unpin(_onFramebufferResize);
                glfw.GcUtility.Unpin(_onFileDrop);
                glfw.GcUtility.Unpin(_onFocusChanged);
            }
        }


        public void Show()
        {
            glfw.ShowWindow(pWindowHandle);
        }

        public void Dispose()
        {
            glfw.DestroyWindow(pWindowHandle);
        }
    }



}
