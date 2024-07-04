// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using win32 = global::Windows.Win32;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.UI.WindowsAndMessaging.WNDCLASS_STYLES;
using static Xultaik.Desktop.WindowProcessEvents;

namespace Xultaik.Desktop;

internal static unsafe class RegisterWin32Class
{
    public static RegisterParams Register(WindowResourcePtr iconResPtr)
    {
        var winParams = new RegisterParams(Win32Helper.GetDefaultModule(), Win32Helper.GenerateHash());

        fixed (char* lpszClassName = winParams.HashName)
        {
            PCWSTR szCursorName = new((char*)PInvoke.IDC_ARROW);

            var wndClassEx = new WNDCLASSEXW
            {
                lpszClassName = lpszClassName,
                cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
                style = CS_CLASSDC,
                lpfnWndProc = &ProccessMainWindowEvents,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = winParams.HInstance,
                hIcon = new HICON(iconResPtr),
                hIconSm = HICON.Null,
                //hCursor = PInvoke.LoadCursor((HINSTANCE)IntPtr.Zero, szCursorName),
                hCursor = HCURSOR.Null,
                hbrBackground = (win32.Graphics.Gdi.HBRUSH)IntPtr.Zero,
                lpszMenuName = null,
            };

            if (PInvoke.RegisterClassEx(&wndClassEx) is 0)
            {
                throw new InvalidOperationException($"Failed to register window class. Error::{Marshal.GetLastWin32Error()}");
            }
        }


        return winParams;
    }

    public static void Unregister(RegisterParams registerParams)
    {
        PInvoke.UnregisterClass(registerParams.HashName, registerParams.Handler);
    }
}
