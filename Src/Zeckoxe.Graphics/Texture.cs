// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Texture.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vulkan;
using Zeckoxe.Core;
using static Vulkan.VulkanNative;

namespace Zeckoxe.Graphics
{
    public unsafe class Texture : GraphicsResource
    {
        public TextureDescription Description { get; set; }

        public TextureDimension Dimension
        {
            get => Description.Dimension;
        }

        public PixelFormat ViewFormat
        {
            get => Description.Format;
        }

        public int MipLevels
        {
            get => Description.MipLevels;
        }

        public int ArraySize
        {
            get => Description.ArraySize;
        }

        public int Width
        {
            get => Description.Width;
        }

        public int Height
        {
            get => Description.Height;
        }

        public int Depth
        {
            get => Description.Depth;
        }

        public PixelFormat Format
        {
            get => Description.Format;
        }




        public Texture(GraphicsDevice device) : base(device)
        {
            Recreate();
        }


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
