// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PixelFormatExtensions.cs
=============================================================================*/


using System;
using System.Collections.Generic;

namespace Zeckoxe.Graphics
{
	public static class PixelFormatExtensions
	{
		// -> https://khronos.org/registry/OpenGL/specs/gl/glspec44.core.pdf
		public static readonly Dictionary<uint, PixelFormat> GLFormatToPixelFormat = new Dictionary<uint, PixelFormat>()
		{
			// -> 8 bits per component
			[0x8229] = PixelFormat.R8UNorm,                 //  1-component, 8-bit unsigned normalized,      GL_R8
			[0x8F94] = PixelFormat.R8G8B8A8UNorm,           //  1-component, 8-bit signed normalized ,       GL_R8_SNORM, 
			[0x8F95] = PixelFormat.R8G8SNorm,               //  2-component, 8-bit signed normalized ,       GL_RG8_SNORM, 
			[0x8F96] = PixelFormat.R8G8B8SNorm,             //  3-component, 8-bit signed normalized ,       GL_RGB8_SNORM, 


			// ->  16 bits per component


			// -> 32 bits per component


			// -> Packed


			// -> S3TC/DXT/BC


			// -> ETC


			// -> PVRTC


			//-> ASTC


			// -> ATC


			// -> Palletized


			// -> Depth/stencil

		};


		public static readonly List<PixelFormat> DepthFormats = new List<PixelFormat>()
		{
			PixelFormat.D32SFloatS8UInt,
			PixelFormat.D32SFloat,
			PixelFormat.D24UNormS8UInt,
			PixelFormat.D16UNormS8UInt,
			PixelFormat.D16UNorm,
		};


		public static bool IsSRGB(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.A8B8G8R8SRgbPack32:
				case PixelFormat.R8G8B8A8SRgb:
				case PixelFormat.B8G8R8A8SRgb:
				case PixelFormat.R8SRgb:
				case PixelFormat.R8G8SRgb:
				case PixelFormat.R8G8B8SRgb:
				case PixelFormat.B8G8R8SRgb:
					return true;

				default:
					return false;
			}
		}


		public static bool HasDepthAspect(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.D16UNorm:
				case PixelFormat.D16UNormS8UInt:
				case PixelFormat.D24UNormS8UInt:
				case PixelFormat.D32SFloat:
				case PixelFormat.X8D24UNormPack32:
				case PixelFormat.D32SFloatS8UInt:
					return true;

				default:
					return false;
			}
		}


		public static bool HasStencilAspect(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.D16UNormS8UInt:
				case PixelFormat.D24UNormS8UInt:
				case PixelFormat.D32SFloatS8UInt:
				case PixelFormat.S8UInt:
					return true;

				default:
					return false;
			}
		}


		public static bool HasDepthOrStencilAspect(this PixelFormat format)
		{
			return HasDepthAspect(format) || HasStencilAspect(format);
		}



		public static int GetFormatVertexSize(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.R8UNorm:
					return 4;

				case PixelFormat.R16UNorm:
					return 4;

				case PixelFormat.R16G16UNorm:
					return 4;

				case PixelFormat.R16G16B16UNorm:
					return 8;

				default:
					return 0;
			}
		}


		public static uint GetImageFormatPixelSize(this PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.R4G4UNormPack8:
					return 1;

				case PixelFormat.R4G4B4A4UNormPack16:
					return 2;

				default:
					throw new ArgumentOutOfRangeException("Format not handled, bug");
			}
		}



		//-> https://www.khronos.org/registry/DataFormat/specs/1.1/dataformat.1.1.pdf
		public static void GetCompressedImageFormatBlockDimensions(this PixelFormat format, out uint w, out uint h)
		{
			w = h = 1;
		}

		public static PixelFormat GetFormatFromOpenGLFormat(uint internalFormat)
		{

			if (!GLFormatToPixelFormat.TryGetValue(internalFormat, out PixelFormat format))
			{
				throw new NotImplementedException("Format not handled, bug");
			}


			return format;
		}
	}
}
