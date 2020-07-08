// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Texture.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Vulkan;
using Zeckoxe.Core;


namespace Zeckoxe.Graphics
{
    public unsafe class Texture : GraphicsResource
    {
        private struct DepthStencil
        {
            public VkImage Image;
            public VkDeviceMemory Mem;
            public VkImageView View;
        }

        internal VkImage Image;
        internal VkDeviceMemory Mem;
        internal VkImageView View;
        //internal DepthStencil depthStencil_;



        public Texture(GraphicsDevice device) : base(device)
        {
            Recreate();
        }


        public TextureDescription Description { get; set; }
        public TextureDimension Dimension => Description.Dimension;
        public PixelFormat ViewFormat => Description.Format;
        public int MipLevels=> Description.MipLevels;
        public int ArraySize => Description.ArraySize;
        public int Width => Description.Width;
        public int Height => Description.Height;
        public int Depth => Description.Depth;
        public PixelFormat Formatt => Description.Format;
        



        public void Recreate()
        {

        }


        public void Imple(DataBox dataBox)
        {
            var pixelsPtr = IntPtr.Zero;

            //Interop.MemoryHelper.Copy(pixelsPtr, dataBox.DataPointer, dataBox.SlicePitch);
        }


    }
}
