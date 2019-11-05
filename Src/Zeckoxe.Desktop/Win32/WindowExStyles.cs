// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	WindowExStyles.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Desktop.Win32
{
    [Flags]
    public enum WindowExStyles : uint
    {
        Left = 0x00000000,

        Trreading = 0x00000000,

        RightScrollbar = 0x00000000,

        DLGModalFrame = 0x00000001,

        NoParentNoTify = 0x00000004,

        TopMost = 0x00000008,

        AcceptFiles = 0x00000010,

        TRANSPARENT = 0x00000020,

        MDIChild = 0x00000040,

        ToolWindow = 0x00000080,

        WindowEdge = 0x00000100,

        PaletteWindow = WindowEdge | ToolWindow | TopMost,

        Clientedge = 0x00000200,

        OverLappedWindow = WindowEdge | Clientedge,

        ContexThelp = 0x00000400,

        Right = 0x00001000,

        RTLReading = 0x00002000,

        LeftScrollBar = 0x00004000,

        ContrOlParent = 0x00010000,

        statiCedge = 0x00020000,

        AppWindow = 0x00040000,

        Layered = 0x00080000,

        NiInheritLayout = 0x00100000,

        NoReDirectionBitmap = 0x00200000,

        LayoutRTL = 0x00400000,

        Composited = 0x02000000,

        NoActivate = 0x08000000
    }
}
