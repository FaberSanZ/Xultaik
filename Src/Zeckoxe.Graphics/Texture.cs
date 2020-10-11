// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture.cs
=============================================================================*/


using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Zeckoxe.Graphics
{
    // TODO: Vulkan Memory Allocator
    public unsafe class Texture : GraphicsResource
    {
        public Texture(Device device) : base(device)
        {
        }


        public TextureDescription Description { get; set; }
        public ImageDimension Dimension => Description.Dimension;
        public PixelFormat ViewFormat => Description.Format;
        public int MipLevels => Description.MipLevels;
        public int ArraySize => Description.ArraySize;
        public int Width => Description.Width;
        public int Height => Description.Height;
        public int Depth => Description.Depth;
        public PixelFormat Formatt => Description.Format;

    }
}
