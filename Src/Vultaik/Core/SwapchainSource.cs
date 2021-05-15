using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public abstract class SwapchainSource
    {
        internal SwapchainSource() { }


        public static SwapchainSource CreateWin32(IntPtr hwnd, IntPtr hinstance) => new Win32SwapchainSource(hwnd, hinstance);
        public static SwapchainSource CreateWindow(ulong surface) => new WindowSwapchainSource(surface);


        public static SwapchainSource CreateXlib(IntPtr display, IntPtr window) => new XlibSwapchainSource(display, window);


        public static SwapchainSource CreateWayland(IntPtr display, IntPtr surface) => new WaylandSwapchainSource(display, surface);

        public static SwapchainSource CreateNSWindow(IntPtr nsWindow) => new NSWindowSwapchainSource(nsWindow);


        public static SwapchainSource CreateUIView(IntPtr uiView) => new UIViewSwapchainSource(uiView);


        public static SwapchainSource CreateAndroidSurface(IntPtr surfaceHandle, IntPtr jniEnv)
            => new AndroidSurfaceSwapchainSource(surfaceHandle, jniEnv);

        public static SwapchainSource CreateNSView(IntPtr nsView)
            => new NSViewSwapchainSource(nsView);
    }



    public class Win32SwapchainSource : SwapchainSource
    {
        public IntPtr Hwnd { get; }
        public IntPtr Hinstance { get; }

        public Win32SwapchainSource(IntPtr hwnd, IntPtr hinstance)
        {
            Hwnd = hwnd;
            Hinstance = hinstance;
        }
    }



    public class WindowSwapchainSource : SwapchainSource
    {
        public ulong Surface { get; }

        public WindowSwapchainSource(ulong surface)
        {
            Surface = surface;
        }
    }


    public class XlibSwapchainSource : SwapchainSource
    {
        public IntPtr Display { get; }
        public IntPtr Window { get; }

        public XlibSwapchainSource(IntPtr display, IntPtr window)
        {
            Display = display;
            Window = window;
        }
    }

    public class WaylandSwapchainSource : SwapchainSource
    {
        public IntPtr Display { get; }
        public IntPtr Surface { get; }

        public WaylandSwapchainSource(IntPtr display, IntPtr surface)
        {
            Display = display;
            Surface = surface;
        }
    }

    public class NSWindowSwapchainSource : SwapchainSource
    {
        public IntPtr NSWindow { get; }

        public NSWindowSwapchainSource(IntPtr nsWindow)
        {
            NSWindow = nsWindow;
        }
    }

    public class UIViewSwapchainSource : SwapchainSource
    {
        public IntPtr UIView { get; }

        public UIViewSwapchainSource(IntPtr uiView)
        {
            UIView = uiView;
        }
    }

    public class AndroidSurfaceSwapchainSource : SwapchainSource
    {
        public IntPtr Surface { get; }
        public IntPtr JniEnv { get; }

        public AndroidSurfaceSwapchainSource(IntPtr surfaceHandle, IntPtr jniEnv)
        {
            Surface = surfaceHandle;
            JniEnv = jniEnv;
        }
    }

    public class NSViewSwapchainSource : SwapchainSource
    {
        public IntPtr NSView { get; }

        public NSViewSwapchainSource(IntPtr nsView)
        {
            NSView = nsView;
        }
    }
}
