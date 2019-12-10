// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	TextureData.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Core;

namespace Zeckoxe.Graphics
{
    public class TextureData
    {
        public byte[] Data { get; internal set; } = Array.Empty<Byte>();

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Depth { get; private set; }

        public int MipMaps { get; private set; }

        public int Size { get; set; }

        public bool IsCubeMap { get; private set; }

        public PixelFormat   Format { get; private set; }



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
        }

    }
}
