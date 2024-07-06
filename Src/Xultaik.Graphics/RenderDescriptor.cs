// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Xultaik.Graphics
{
    public struct RenderDescriptor
    {

        public PixelFormat BackBufferFormat { get; set; }

        public int BackBufferWidth { get; set; }

        public int BackBufferHeight { get; set; }

        public IntPtr DeviceHandle { get; set; }

        public Settings Settings { get; set; }

    }
}