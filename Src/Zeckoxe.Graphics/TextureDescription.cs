// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	TextureDescription.cs
=============================================================================*/

namespace Zeckoxe.Graphics
{
    public class TextureDescription
    {
        public TextureDimension Dimension { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int ArraySize { get; set; }

        public int MipLevels { get; set; }

        public PixelFormat Format { get; set; }

        public GraphicsResourceUsage Usage { get; set; }
    }
}