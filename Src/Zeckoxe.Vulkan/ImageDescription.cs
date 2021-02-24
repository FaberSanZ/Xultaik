// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	TextureDescription.cs
=============================================================================*/

using System;
using Vortice.Vulkan;

namespace Zeckoxe.Vulkan
{
    public enum ImageDimension
    {
        Image1D,

        Image2D,

        Image3D,

        ImageCube
    }

    public class ImageData //: ITextureData
    {
        public byte[] Data { get; internal set; } = Array.Empty<Byte>();

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int MipMaps { get; set; }

        public int Size { get; set; }

        public bool IsCubeMap { get; set; }

        public VkFormat Format { get; set; }
        //byte[] ITextureData.Data { get => Data; set => Data = value; }

        public ImageData(byte[] data, int width, int height, int depth, int mipMaps, int size, bool isCubeMap, VkFormat format)
        {
            Data = data;
            Width = width;
            Height = height;
            Depth = depth;
            MipMaps = mipMaps;
            Size = size;
            IsCubeMap = isCubeMap;
            Format = format;

            //ITextureData d = new TextureData();
            //d.Data
        }


        public ImageData()
        {
            Data = Array.Empty<byte>();
            Width = 0;
            Height = 0;
            Depth = 0;
            MipMaps = 0;
            Size = 0;
            IsCubeMap = false;
            Format = VkFormat.Undefined;
        }





        //public override string ToString() => string.Format("Data:{0} Width:{1} Depth:{0} Width:{1} Data:{0} Width:{1}", Data.Length, Width, Height, Depth, MipMaps, Size, IsCubeMap, Format);


    }


    public class ImageDescription
    {
        public ImageDimension Dimension { get; set; }

        public byte[] Data { get; set; } = Array.Empty<byte>();

        public int MipMaps { get; set; }

        public int Size { get; set; }

        public bool IsCubeMap { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int ArraySize { get; set; }

        public int MipLevels { get; set; }

        public VkFormat Format { get; set; }

        public GraphicsResourceUsage Usage { get; set; }

        public TextureFlags Flags { get; set; }


        internal VkFormat format { get; set; }
    }
}