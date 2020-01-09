// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	FillMode.cs
=============================================================================*/

namespace Zeckoxe.Graphics
{
    public enum FillMode : int
    {
        Solid = unchecked(0),

        Wireframe = unchecked(1),

        Point = unchecked(2),

        FillRectangleNV = unchecked(1000153000)
    }
}