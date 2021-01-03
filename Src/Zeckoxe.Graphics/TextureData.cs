// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	TextureData.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public interface ITextureData
    {
        public byte[] Data { get; internal set; }
        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int MipMaps { get; set; }

        public int Size { get; set; }

        public bool IsCubeMap { get; set; }

        public PixelFormat Format { get; set; }
    }

    public class TextureData //: ITextureData
    {
        public byte[] Data { get; internal set; } = Array.Empty<Byte>();

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int MipMaps { get; set; }

        public int Size { get; set; }

        public bool IsCubeMap { get; set; }

        public PixelFormat   Format { get; set; }
        //byte[] ITextureData.Data { get => Data; set => Data = value; }

        public TextureData(byte[] data, int width, int height, int depth, int mipMaps, int size, bool isCubeMap, PixelFormat format)
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


        public TextureData()
        {
            Data = Array.Empty<byte>();
            Width = 0;
            Height = 0;
            Depth = 0;
            MipMaps = 0;
            Size = 0;
            IsCubeMap = false;
            Format = PixelFormat.Undefined;
        }





        //public override string ToString() => string.Format("Data:{0} Width:{1} Depth:{0} Width:{1} Data:{0} Width:{1}", Data.Length, Width, Height, Depth, MipMaps, Size, IsCubeMap, Format);


    }
}
