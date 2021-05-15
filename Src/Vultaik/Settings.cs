// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Settings.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Vultaik
{
    public class Settings
    {
        public ValidationType Validation { get; set; }

        public OptionalDeviceExtensions OptionalDeviceExtensions { get; set; }

        public bool Fullscreen { get; set; }

        public bool VSync { get; set; }

    }
}
