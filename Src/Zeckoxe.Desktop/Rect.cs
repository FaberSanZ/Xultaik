// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Rect.cs
=============================================================================*/

namespace Zeckoxe.Desktop
{
    public struct Rect
    {
        public int Left, Top, Right, Bottom;

        public Rect(int Left, int Top, int Right, int Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }
    }
}
