// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	GraphicsAdapter.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public enum DeviceType : int
    {
        Other = unchecked(0),

        IntegratedGPU = unchecked(1),

        DiscreteGPU = unchecked(2),

        VirtualGPU = unchecked(3),

        CPU = unchecked(4)
    }
}
