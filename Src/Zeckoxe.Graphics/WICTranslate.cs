// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	WICTranslate.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Vortice.WIC;

namespace Zeckoxe.Graphics
{
    public class WICTranslate
    {
        public static IWICImagingFactory Factory => new IWICImagingFactory();


        internal Dictionary<Guid, Vortice.DXGI.Format> Translate = new Dictionary<Guid, Vortice.DXGI.Format>();

        public Dictionary<Guid, Vortice.DXGI.Format> Element => Translate;



        public WICTranslate()
        {
            Translate.Add(Vortice.WIC.PixelFormat.Format128bppRGBAFloat, Vortice.DXGI.Format.R32G32B32A32_Float);

            Translate.Add(Vortice.WIC.PixelFormat.Format64bppRGBAHalf, Vortice.DXGI.Format.R16G16B16A16_Float);
            Translate.Add(Vortice.WIC.PixelFormat.Format64bppRGBA, Vortice.DXGI.Format.R16G16B16A16_UNorm);

            Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA, Vortice.DXGI.Format.R8G8B8A8_UNorm);
            Translate.Add(Vortice.WIC.PixelFormat.Format32bppBGRA, Vortice.DXGI.Format.B8G8R8A8_UNorm);
            Translate.Add(Vortice.WIC.PixelFormat.Format32bppBGR, Vortice.DXGI.Format.B8G8R8X8_UNorm);

            Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA1010102XR, Vortice.DXGI.Format.R10G10B10_Xr_Bias_A2_UNorm);
            Translate.Add(Vortice.WIC.PixelFormat.Format32bppRGBA1010102, Vortice.DXGI.Format.R10G10B10A2_UNorm);

            Translate.Add(Vortice.WIC.PixelFormat.Format16bppBGRA5551, Vortice.DXGI.Format.B5G5R5A1_UNorm);
            Translate.Add(Vortice.WIC.PixelFormat.Format16bppBGR565, Vortice.DXGI.Format.B5G6R5_UNorm);

            Translate.Add(Vortice.WIC.PixelFormat.Format32bppGrayFloat, Vortice.DXGI.Format.R32_Float);
            Translate.Add(Vortice.WIC.PixelFormat.Format16bppGrayHalf, Vortice.DXGI.Format.R16_Float);
            Translate.Add(Vortice.WIC.PixelFormat.Format16bppGray, Vortice.DXGI.Format.R16_UNorm);
            Translate.Add(Vortice.WIC.PixelFormat.Format8bppGray, Vortice.DXGI.Format.R8_UNorm);

            Translate.Add(Vortice.WIC.PixelFormat.Format8bppAlpha, Vortice.DXGI.Format.A8_UNorm);
        }




        public Vortice.DXGI.Format this[Guid index]
        {
            get
            {
                if (Translate.ContainsKey(index) is false)
                    return Vortice.DXGI.Format.Unknown;

                return Translate[index];
            }
        }
    }
}
