// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Win32Native.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zeckoxe.Desktop
{
    internal unsafe static partial class Win32Native
    {
        public const int GCS_COMPSTR = 0x0008;
        public const int WM_SIZE = 0x0005;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_POWERBROADCAST = 0x0218;
        public const int WM_MENUCHAR = 0x0120;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int WM_INPUTLANGCHANGE = 0x0051;
        public const int WM_IME_CHAR = 0x0286;
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int WM_IME_COMPOSITIONFULL = 0x0284;
        public const int WM_IME_CONTROL = 0x0283;
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        public const int WM_IME_KEYDOWN = 0x0290;
        public const int WM_IME_KEYLAST = 0x010F;
        public const int WM_IME_KEYUP = 0x0291;
        public const int WM_IME_NOTIFY = 0x0282;
        public const int WM_IME_REQUEST = 0x0288;
        public const int WM_IME_SELECT = 0x0285;
        public const int WM_IME_SETCONTEXT = 0x0281;
        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        public const int WM_PAINT = 0x000F;
        public const int WM_NCPAINT = 0x0085;
        public const int PM_REMOVE = 0x0001;


        public const string DllName = "user32.dll";




        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeMessage
        {
            public void* handle;
            public uint msg;
            public void* wParam;
            public void* lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;
        }




        [DllImport(DllName)]
        public static extern int PeekMessage(NativeMessage* lpMsg, void* hWnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg);



        [DllImport(DllName)]
        public static extern int TranslateMessage(NativeMessage* lpMsg);



        [DllImport(DllName)]
        public static extern int DispatchMessage(NativeMessage* lpMsg);


    }
}
