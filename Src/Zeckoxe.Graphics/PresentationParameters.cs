// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PresentationParameters.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class PresentationParameters
    {
        public int BackBufferWidth { get; set; }

        public int BackBufferHeight { get; set; }

        public SwapchainSource SwapchainSource { get; set; }

        public Settings Settings { get; set; }
    }



}
