// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	IMGLoader.cs
=============================================================================*/





using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Zeckoxe.Core;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp.Formats;

namespace Zeckoxe.Image
{

	public unsafe class IMGLoader
	{
        internal Image<Rgba32>[] Images { get; set; }

        public TextureData TextureData { get; private set; }



        public IMGLoader(string filename)
        {
            TextureData = LoadTexture(filename);
        }



        public static TextureData LoadFromFile(string filename) => new IMGLoader(filename).TextureData;


        internal TextureData LoadTexture(string filename)
        {
            Images = GenerateMipmaps(SixLabors.ImageSharp.Image.Load<Rgba32>(filename));

            Span<byte> pixels;

            fixed (void* pointer = Images[0x0].GetPixelSpan())
                pixels = new Span<byte>(pointer, Images[0x0].GetPixelSpan().Length);

            return new TextureData()
            {
                MipMaps = Images.Length,
                Format = PixelFormat.R8G8B8A8_UNorm,
                Width = Images[0x0].Width,
                Height = Images[0x0].Height,
                Depth = 0x1,
                Size = Images[0x0].Width * Interop.SizeOf<Rgba32>(),//+ 7 / 8,
                Data = pixels.ToArray(),
                IsCubeMap = false,
            };
        }



        internal Image<T>[] GenerateMipmaps<T>(Image<T> baseImage) where T : struct, IPixel<T>
        {
            int mipLevelCount = (int)Math.Floor(Math.Log(Math.Max(baseImage.Width, baseImage.Height), 2)) + 1;
            int i = 1;

            Image<T>[] mipLevels = new Image<T>[mipLevelCount];
            mipLevels[0x0] = baseImage;


            int currentWidth = baseImage.Width;
            int currentHeight = baseImage.Height; 

            while (currentWidth != 1 || currentHeight != 1)
            {
                int newWidth = Math.Max(1, currentWidth / 2);
                int newHeight = Math.Max(1, currentHeight / 2);

                Image<T> newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));

                mipLevels[i] = newImage;

                i++;

                currentWidth = newWidth;
                currentHeight = newHeight;
            }


            return mipLevels;
        }

    }
}
