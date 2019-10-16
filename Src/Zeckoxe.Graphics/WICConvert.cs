// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	WICConvert.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.WIC;

namespace Zeckoxe.Graphics
{
    public class WICConvert
    {
        private Dictionary<Guid, Guid> Convert = new Dictionary<Guid, Guid>();

        public Dictionary<Guid, Guid> Element => Convert;


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
}
