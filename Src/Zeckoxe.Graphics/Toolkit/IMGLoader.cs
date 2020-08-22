// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	IMGLoader.cs
=============================================================================*/




using System;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;


namespace Zeckoxe.Graphics.Toolkit
{

    public unsafe class IMGLoader
    {

        public IMGLoader(string filename)
        {
            using Image<Rgba32> image = Image.Load<Rgba32>(filename);

            Span<Rgba32> pixels = image.GetPixelSpan();

            //for (int i = 0; i < pixels.Length; i++)
            //{
            //    ref Rgba32 pixel = ref pixels[i];
            //    byte a = pixel.A;
            //
            //    if (a is 0)
            //    {
            //        pixel.PackedValue = 0;
            //    }
            //    else
            //    {
            //        pixel.R = (byte)((pixel.R * a) >> 8);
            //        pixel.G = (byte)((pixel.G * a) >> 8);
            //        pixel.B = (byte)((pixel.B * a) >> 8);
            //    }
            //}

            TextureData data = new TextureData()
            {
                Width = image.Width,
                Height = image.Height,
                Format = PixelFormat.R8G8B8A8UNorm,
                Size = 4,
                Depth = 1,
                IsCubeMap = false,
                MipMaps = 1, // TODO: MipMaps 
                Data = MemoryMarshal.AsBytes(pixels).ToArray(),
            };


            TextureData = data;

        }


        public TextureData TextureData { get; private set; }


        public static TextureData LoadFromFile(string filename)
        {
            return new IMGLoader(filename).TextureData;
        }
    }
}
