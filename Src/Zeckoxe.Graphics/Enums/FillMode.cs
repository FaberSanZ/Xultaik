// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


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

        FillRectangleNV = unchecked(3)
    }
}