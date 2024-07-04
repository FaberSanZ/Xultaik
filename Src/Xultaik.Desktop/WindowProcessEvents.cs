// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Xultaik.Desktop;

public static class WindowProcessEvents
{
    public unsafe delegate IntPtr ExternalWindowDelegate(void* windowHandlePtr, uint msg, IntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void WindowDelegate(HWND windowHandle, uint message, WPARAM wParam, LPARAM lParam);


    private readonly static Dictionary<HWND, WindowDelegate> ProcEvents = [];

    internal static void Include(Window window)
    {
        ProcEvents.Add(window.handler, window.WindowDelegateEvent);
    }

    internal static void Remove(Window window)
    {
        ProcEvents.Remove(window.handler);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static LRESULT ProccessMainWindowEvents(HWND currentHandleWindow, uint message, WPARAM wParam, LPARAM lParam)
    {
        foreach (var proc in ProcEvents)
        {
            HWND hWnd = proc.Key;

            if (hWnd == currentHandleWindow.Value)
            {
                ProcEvents[proc.Key].Invoke(currentHandleWindow, message, wParam, lParam);
            }
        }

        return PInvoke.DefWindowProc(currentHandleWindow, message, wParam, lParam);
    }
}
