// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	WICLoader.cs
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

	public class IMGLoader
	{
        internal Image<Rgba32>[] Images { get; set; }



        public unsafe TextureData CreateData(string path)
        {
            Images = GenerateMipmaps<Rgba32>(SixLabors.ImageSharp.Image.Load<Rgba32>(path));

            Span<byte> pixels;

            fixed (void* pointer = Images[0].GetPixelSpan())
                pixels = new Span<byte>(pointer, Images[0].GetPixelSpan().Length);


            TextureData data = new TextureData();
            data.MipMaps = Images.Length;
            data.Format = PixelFormat.R8G8B8A8_UNorm;
            data.Width = Images[0].Width;
            data.Height = Images[0].Height;
            data.Depth = 1;
            data.Size = Images[0].Width * Interop.SizeOf<Rgba32>();
            data.Data = pixels.ToArray().AsMemory();

            

            return data;

        }



        public int ComputeMipLevels(int width, int height)
        {
            return 1 + (int)Math.Floor(Math.Log(Math.Max(width, height), 2));
        }


        public Image<T>[] GenerateMipmaps<T>(Image<T> baseImage) where T : struct, IPixel<T>
        {
            int mipLevelCount = ComputeMipLevels(baseImage.Width, baseImage.Height);
            Image<T>[] mipLevels = new Image<T>[mipLevelCount];
            mipLevels[0] = baseImage;
            int i = 1;

            int currentWidth = baseImage.Width;
            int currentHeight = baseImage.Height;
            while (currentWidth != 1 || currentHeight != 1)
            {
                int newWidth = Math.Max(1, currentWidth / 2);
                int newHeight = Math.Max(1, currentHeight / 2);

                Image<T> newImage = baseImage.Clone(context => context.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));
                Debug.Assert(i < mipLevelCount);
                mipLevels[i] = newImage;

                i++;
                currentWidth = newWidth;
                currentHeight = newHeight;
            }

            Debug.Assert(i == mipLevelCount);

            return mipLevels;
        }

        public static TextureData LoadFromFile(string v)
        {
            return new IMGLoader().CreateData(v);
        }
    }
}
