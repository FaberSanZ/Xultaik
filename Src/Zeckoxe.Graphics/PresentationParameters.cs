// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.
// Copyright (c) 2019-2020 Zeckoxe Engine contributors (cf. AUTHORS.md)

/*=============================================================================
	PresentationParameters.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public struct PresentationParameters
    {

        public PixelFormat BackBufferFormat { get; set; }

        public int BackBufferWidth { get; set; }

        public int BackBufferHeight { get; set; }

        public IntPtr DeviceHandle { get; set; }

        public Settings Settings { get; set; }

    }



}