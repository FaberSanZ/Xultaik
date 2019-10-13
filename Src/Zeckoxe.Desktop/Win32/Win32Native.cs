// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Win32Native.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.Desktop.Win32
{
    public static unsafe class Win32Native
    {
        const string User32 = "user32.dll";

        const string kernel32 = "kernel32.dll";


        [DllImport(kernel32)]
        public static extern IntPtr LoadLibrary(string fileName);



        [DllImport(kernel32, CharSet = CharSet.Ansi, BestFitMapping = false)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);



        [DllImport(kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern bool FreeLibrary([In] IntPtr hModule);



        [DllImport(kernel32)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);



        [DllImport(kernel32)]
        public static extern IntPtr GetCurrentProcess();



        [DllImport(kernel32)]
        public static extern IntPtr GetCurrentThread();



        [return: MarshalAs(UnmanagedType.U1)]
        [DllImport(kernel32, SetLastError = true)]
        public static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask);



        [DllImport(kernel32)]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);




        /*****************************************************************************/
        /*----------------------------------User32-----------------------------------*/

        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);



        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(User32)]
        public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr CallWindowProc(WNDPROC lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, SystemCursor lpCursorResource);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern int GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);



        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(User32, CharSet = CharSet.Unicode, EntryPoint = "PeekMessageW")]
        public static extern bool PeekMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);



        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "PostMessageW")]
        public static extern bool PostMessage(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);



        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(User32, ExactSpelling = true)]
        public static extern bool TranslateMessage([In] ref Message lpMsg);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr DispatchMessage([In] ref Message lpmsg);



        [DllImport(User32, ExactSpelling = true)]
        public static extern int GetSystemMetrics(SystemMetrics smIndex);



        [DllImport(User32, SetLastError = true)]
        private static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);



        [DllImport(User32, SetLastError = true, EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong32b(IntPtr hWnd, int nIndex);



        [DllImport(User32, SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);



        [DllImport(User32, SetLastError = true)]
        private static extern uint SetWindowLongPtr(IntPtr hWnd, int nIndex, uint value);



        [return: MarshalAs(UnmanagedType.U1)]
        [DllImport(User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRect(Rect* lpRect, WindowStyles dwStyle, bool hasMenu);



        [return: MarshalAs(UnmanagedType.U1)]
        [DllImport(User32, ExactSpelling = true)]
        public static extern bool AdjustWindowRectEx(Rect* lpRect, WindowStyles dwStyle, bool bMenu, WindowExStyles exStyle);



        [DllImport(User32, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx
                            (WindowExStyles exStyle, string className, string windowName, 
                             WindowStyles style, int x, int y, int width, int height, IntPtr 
                             hwndParent, IntPtr Menu, IntPtr Instance, IntPtr pvParam);



        [DllImport(User32, ExactSpelling = true)]
        public static extern bool DestroyWindow(IntPtr windowHandle);



        [DllImport(User32, ExactSpelling = true)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);



        [DllImport(User32)]
        public static extern void PostQuitMessage(int nExitCode);
    }
}
