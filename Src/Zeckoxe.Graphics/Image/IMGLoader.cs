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
using Vortice.WIC;
using PixelFormat = Zeckoxe.Graphics.PixelFormat;

namespace Zeckoxe.Image
{

	public unsafe class IMGLoader
	{
        internal class WICConvert
        {
            private Dictionary<Guid, Guid> Convert = new Dictionary<Guid, Guid>();

            public WICConvert()
            {
                Convert.Add(Vortice.WIC.PixelFormat.FormatBlackWhite, Vortice.WIC.PixelFormat.Format8bppGray);
                Convert.Add(Vortice.WIC.PixelFormat.Format1bppIndexed, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format2bppIndexed, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format4bppIndexed, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format8bppIndexed, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format2bppGray, Vortice.WIC.PixelFormat.Format8bppGray);
                Convert.Add(Vortice.WIC.PixelFormat.Format4bppGray, Vortice.WIC.PixelFormat.Format8bppGray);
                Convert.Add(Vortice.WIC.PixelFormat.Format16bppGrayFixedPoint, Vortice.WIC.PixelFormat.Format16bppGrayHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppGrayFixedPoint, Vortice.WIC.PixelFormat.Format32bppGrayFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format16bppBGR555, Vortice.WIC.PixelFormat.Format16bppBGRA5551);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppBGR101010, Vortice.WIC.PixelFormat.Format32bppRGBA1010102);
                Convert.Add(Vortice.WIC.PixelFormat.Format24bppBGR, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format24bppRGB, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppPBGRA, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppPRGBA, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format48bppRGB, Vortice.WIC.PixelFormat.Format48bppRGB);
                Convert.Add(Vortice.WIC.PixelFormat.Format48bppBGR, Vortice.WIC.PixelFormat.Format64bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format64bppRGBAFixedPoint, Vortice.WIC.PixelFormat.Format64bppRGBAHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format64bppBGRAFixedPoint, Vortice.WIC.PixelFormat.Format64bppRGBAHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format64bppRGBFixedPoint, Vortice.WIC.PixelFormat.Format64bppRGBAHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format64bppRGBHalf, Vortice.WIC.PixelFormat.Format64bppRGBAHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format48bppRGBHalf, Vortice.WIC.PixelFormat.Format64bppRGBAHalf);
                Convert.Add(Vortice.WIC.PixelFormat.Format128bppPRGBAFloat, Vortice.WIC.PixelFormat.Format128bppRGBAFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format128bppRGBFloat, Vortice.WIC.PixelFormat.Format128bppRGBAFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format128bppRGBAFixedPoint, Vortice.WIC.PixelFormat.Format128bppRGBAFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format128bppRGBFixedPoint, Vortice.WIC.PixelFormat.Format128bppRGBAFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppRGBE, Vortice.WIC.PixelFormat.Format128bppRGBAFloat);
                Convert.Add(Vortice.WIC.PixelFormat.Format32bppCMYK, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format64bppCMYK, Vortice.WIC.PixelFormat.Format64bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format40bppCMYKAlpha, Vortice.WIC.PixelFormat.Format32bppRGBA);
                Convert.Add(Vortice.WIC.PixelFormat.Format80bppCMYKAlpha, Vortice.WIC.PixelFormat.Format64bppRGBA);
            }


            public Guid this[Guid index]
            {
                get
                {
                    if (Convert.ContainsKey(index) is false)
                        throw new ArgumentException("Index Vailed");

                    return Convert[index];
                }
            }
        }

        internal class WICTranslate
        {
            internal Dictionary<Guid, PixelFormat> Translate = new Dictionary<Guid, PixelFormat>();

            public WICTranslate()
            {
                Translate.Add(Vortice.WIC.PixelFormat.Format128bppRGBAFloat, PixelFormat.R32G32B32A32_Float);
                Translate.Add(Vortice.WIC.PixelFormat.Format64bppRGBAHalf, PixelFormat.R16G16B16A16_Float);
                Translate.Add(Vortice.WIC.PixelFormat.Format64bppRGBA, PixelFormat.R16G16B16A16_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA, PixelFormat.R8G8B8A8_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppBGRA, PixelFormat.B8G8R8A8_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppBGR, PixelFormat.B8G8R8X8_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA1010102XR, PixelFormat.R10G10B10_Xr_Bias_A2_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA1010102, PixelFormat.R10G10B10A2_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format16bppBGRA5551, PixelFormat.B5G5R5A1_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format16bppBGR565, PixelFormat.B5G6R5_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format32bppGrayFloat, PixelFormat.R32_Float);
                Translate.Add(Vortice.WIC.PixelFormat.Format16bppGrayHalf, PixelFormat.R16_Float);
                Translate.Add(Vortice.WIC.PixelFormat.Format16bppGray, PixelFormat.R16_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format8bppGray, PixelFormat.R8_UNorm);
                Translate.Add(Vortice.WIC.PixelFormat.Format8bppAlpha, PixelFormat.A8_UNorm);
            }

            public PixelFormat this[Guid index]
            {
                get
                {
                    if (Translate.ContainsKey(index) is false)
                        return PixelFormat.Unknown;

                    return Translate[index];
                }
            }
        }



        public TextureData TextureData { get; private set; }


        public IMGLoader(string filename)
        {
            var bitmap = ReadBitmap(filename);

            WICTranslate Translate = new WICTranslate();

            int mipLevelCount = (int)Math.Floor(Math.Log(Math.Max(bitmap.Size.Width, bitmap.Size.Height), 2)) + 1;

            TextureData = new TextureData()
            {
                Height = bitmap.Size.Height,
                Width = bitmap.Size.Width,
                Depth = 1,
                Format = Translate[bitmap.PixelFormat],
                MipMaps = mipLevelCount,
            };


            int bpp = GetBitsPerPixel(TextureData.Format);

            int rowPitch = (TextureData.Width * bpp + 7) / 8;
            int slicePitch = rowPitch * TextureData.Height;

            byte[] data = new byte[slicePitch];

            bitmap.CopyPixels(rowPitch, data);



            // TODO: Size -------
            TextureData.Size = TextureData.Width * 4;  //(slicePitch / rowPitch) * 4;
            //-------------------

            TextureData.Data = data;
        }





        public static TextureData LoadFromFile(string filename) => new IMGLoader(filename).TextureData;

        internal IWICBitmapSource ReadBitmap(string filename)
        {
            using var factory = new IWICImagingFactory2();

            IWICBitmapDecoder bitmapDecoder = factory.CreateDecoderFromFilename(filename, DecodeOptions.CacheOnLoad);

            IWICFormatConverter formatConverter = factory.CreateFormatConverter();

            IWICBitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

            formatConverter.Initialize(frame, Vortice.WIC.PixelFormat.Format32bppRGBA, BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);


            return formatConverter;
        }


        internal int GetBitsPerPixel(PixelFormat dxgiFormat)
        {


            switch (dxgiFormat)
            {
                case PixelFormat.R32G32B32A32_Float:
                    return 128;

                case PixelFormat.R16G16B16A16_Float:
                case PixelFormat.R16G16B16A16_UNorm:
                    return 64;

                case PixelFormat.R8G8B8A8_UNorm:
                case PixelFormat.B8G8R8X8_UNorm:
                case PixelFormat.B8G8R8A8_UNorm:
                case PixelFormat.R32_Float:
                case PixelFormat.R10G10B10_Xr_Bias_A2_UNorm:
                    return 32;



                case PixelFormat.B5G5R5A1_UNorm:
                case PixelFormat.B5G6R5_UNorm:
                case PixelFormat.R16_Float:
                case PixelFormat.R16_UNorm:
                    return 16;


                case PixelFormat.R8_UNorm:
                case PixelFormat.A8_UNorm:
                    return 8;



                default:
                    return 8;


            }

        }


    }
}
