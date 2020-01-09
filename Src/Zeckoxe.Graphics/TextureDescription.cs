// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	TextureDescription.cs
=============================================================================*/

namespace Zeckoxe.Graphics
{
    public struct TextureDescription
    {
        public TextureDimension Dimension;

        public int Width;

        public int Height;

        public int Depth;

        public int ArraySize;

        public int MipLevels;

        public PixelFormat Format;

        public GraphicsResourceUsage Usage;
    }
}