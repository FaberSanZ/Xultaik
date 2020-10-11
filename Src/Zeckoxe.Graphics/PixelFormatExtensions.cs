// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PixelFormatExtensions.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using Vortice.Vulkan;

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


		//const uint GL_R8 = 0x8229;
		//const uint GL_R8_SNORM = 0x8F94;
		//const uint GL_RG8_SNORM = 0x8F95;
		//const uint GL_RGB8_SNORM = 0x8F96;
		//const uint GL_RGBA8_SNORM = 0x8F97;
		//const uint GL_R8UI = 0x8232;
		//const uint GL_RG8UI = 0x8238;
		//const uint GL_RGB8UI = 0x8D7D;
		//const uint GL_RGBA8UI = 0x8D7C;
		//const uint GL_R8I = 0x8231;
		//const uint GL_RG8I = 0x8237;
		//const uint GL_RGB8I = 0x8D8F;
		//const uint GL_RGBA8I = 0x8D8E;
		//const uint GL_SR8 = 0x8FBD;
		//const uint GL_SRG8 = 0x8FBE;
		//const uint GL_SRGB8 = 0x8C41;
		//const uint GL_SRGB8_ALPHA8 = 0x8C43;
		//const uint GL_R16 = 0x822A;
		//const uint GL_RG16 = 0x822C;
		//const uint GL_RGB16 = 0x8054;
		//const uint GL_RGBA16 = 0x805B;
		//const uint GL_R16_SNORM = 0x8F98;
		//const uint GL_RG16_SNORM = 0x8F99;
		//const uint GL_RGB16_SNORM = 0x8F9A;
		//const uint GL_RGBA16_SNORM = 0x8F9B;
		//const uint GL_R16UI = 0x8234;
		//const uint GL_RG16UI = 0x823A;
		//const uint GL_RGB16UI = 0x8D77;
		//const uint GL_RGBA16UI = 0x8D76;
		//const uint GL_R16I = 0x8233;
		//const uint GL_RG16I = 0x8239;
		//const uint GL_RGB16I = 0x8D89;
		//const uint GL_RGBA16I = 0x8D88;
		//const uint GL_R16F = 0x822D;
		//const uint GL_RG16F = 0x822F;
		//const uint GL_RGB16F = 0x881B;
		//const uint GL_RGBA16F = 0x881A;
		//const uint GL_R32UI = 0x8236;
		//const uint GL_RG32UI = 0x823C;
		//const uint GL_RGB32UI = 0x8D71;
		//const uint GL_RGBA32UI = 0x8D70;
		//const uint GL_R32I = 0x8235;
		//const uint GL_RG32I = 0x823B;
		//const uint GL_RGB32I = 0x8D83;
		//const uint GL_RGBA32I = 0x8D82;
		//const uint GL_R32F = 0x822E;
		//const uint GL_RG32F = 0x8230;
		//const uint GL_RGB32F = 0x8815;
		//const uint GL_RGBA32F = 0x8814;
		//const uint GL_R3_G3_B2 = 0x2A10;
		//const uint GL_RGB4 = 0x804F;
		//const uint GL_RGB5 = 0x8050;
		//const uint GL_RGB565 = 0x8D62;
		//const uint GL_RGB10 = 0x8052;
		//const uint GL_RGB12 = 0x8053;
		//const uint GL_RGBA2 = 0x8055;
		//const uint GL_RGBA4 = 0x8056;
		//const uint GL_RGBA12 = 0x805A;
		//const uint GL_RGB5_A1 = 0x8057;
		//const uint GL_RGB10_A2 = 0x8059;
		//const uint GL_RGB10_A2UI = 0x906F;
		//const uint GL_R11F_G11F_B10F = 0x8C3A;
		//const uint GL_RGB9_E5 = 0x8C3D;
		//const uint GL_COMPRESSED_RGB_S3TC_DXT1_EXT = 0x83F0;
		//const uint GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1;
		//const uint GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3;
		//const uint GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2;
		//const uint GL_COMPRESSED_SRGB_S3TC_DXT1_EXT = 0x8C4C;
		//const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT = 0x8C4D;
		//const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT = 0x8C4E;
		//const uint GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT = 0x8C4F;
		//const uint GL_COMPRESSED_LUMINANCE_LATC1_EXT = 0x8C70;
		//const uint GL_COMPRESSED_LUMINANCE_ALPHA_LATC2_EXT = 0x8C72;
		//const uint GL_COMPRESSED_SIGNED_LUMINANCE_LATC1_EXT = 0x8C71;
		//const uint GL_COMPRESSED_SIGNED_LUMINANCE_ALPHA_LATC2_EXT = 0x8C73;
		//const uint GL_COMPRESSED_RED_RGTC1 = 0x8DBB;
		//const uint GL_COMPRESSED_RG_RGTC2 = 0x8DBD;
		//const uint GL_COMPRESSED_SIGNED_RED_RGTC1 = 0x8DBC;
		//const uint GL_COMPRESSED_SIGNED_RG_RGTC2 = 0x8DBE;
		//const uint GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT = 0x8E8F;
		//const uint GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT = 0x8E8E;
		//const uint GL_COMPRESSED_RGBA_BPTC_UNORM = 0x8E8C;
		//const uint GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM = 0x8E8D;
		//const uint GL_ETC1_RGB8_OES = 0x8D64;
		//const uint GL_COMPRESSED_RGB8_ETC2 = 0x9274;
		//const uint GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276;
		//const uint GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278;
		//const uint GL_COMPRESSED_SRGB8_ETC2 = 0x9275;
		//const uint GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279;
		//const uint GL_COMPRESSED_R11_EAC = 0x9270;
		//const uint GL_COMPRESSED_RG11_EAC = 0x9272;
		//const uint GL_COMPRESSED_SIGNED_R11_EAC = 0x9271;
		//const uint GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273;
		//const uint GL_COMPRESSED_RGB_PVRTC_2BPPV1_IMG = 0x8C01;
		//const uint GL_COMPRESSED_RGB_PVRTC_4BPPV1_IMG = 0x8C00;
		//const uint GL_COMPRESSED_RGBA_PVRTC_2BPPV1_IMG = 0x8C03;
		//const uint GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG = 0x8C02;
		//const uint GL_COMPRESSED_RGBA_PVRTC_2BPPV2_IMG = 0x9137;
		//const uint GL_COMPRESSED_RGBA_PVRTC_4BPPV2_IMG = 0x9138;
		//const uint GL_COMPRESSED_SRGB_PVRTC_2BPPV1_EXT = 0x8A54;
		//const uint GL_COMPRESSED_SRGB_PVRTC_4BPPV1_EXT = 0x8A55;
		//const uint GL_COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV1_EXT = 0x8A56;
		//const uint GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV1_EXT = 0x8A57;
		//const uint GL_COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV2_IMG = 0x93F0;
		//const uint GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV2_IMG = 0x93F1;
		//const uint GL_COMPRESSED_RGBA_ASTC_4x4_KHR = 0x93B0;
		//const uint GL_COMPRESSED_RGBA_ASTC_5x4_KHR = 0x93B1;
		//const uint GL_COMPRESSED_RGBA_ASTC_5x5_KHR = 0x93B2;
		//const uint GL_COMPRESSED_RGBA_ASTC_6x5_KHR = 0x93B3;
		//const uint GL_COMPRESSED_RGBA_ASTC_6x6_KHR = 0x93B4;
		//const uint GL_COMPRESSED_RGBA_ASTC_8x5_KHR = 0x93B5;
		//const uint GL_COMPRESSED_RGBA_ASTC_8x6_KHR = 0x93B6;
		//const uint GL_COMPRESSED_RGBA_ASTC_8x8_KHR = 0x93B7;
		//const uint GL_COMPRESSED_RGBA_ASTC_10x5_KHR = 0x93B8;
		//const uint GL_COMPRESSED_RGBA_ASTC_10x6_KHR = 0x93B9;
		//const uint GL_COMPRESSED_RGBA_ASTC_10x8_KHR = 0x93BA;
		//const uint GL_COMPRESSED_RGBA_ASTC_10x10_KHR = 0x93BB;
		//const uint GL_COMPRESSED_RGBA_ASTC_12x10_KHR = 0x93BC;
		//const uint GL_COMPRESSED_RGBA_ASTC_12x12_KHR = 0x93BD;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4_KHR = 0x93D0;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x4_KHR = 0x93D1;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5_KHR = 0x93D2;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x5_KHR = 0x93D3;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6_KHR = 0x93D4;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x5_KHR = 0x93D5;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x6_KHR = 0x93D6;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_8x8_KHR = 0x93D7;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x5_KHR = 0x93D8;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x6_KHR = 0x93D9;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x8_KHR = 0x93DA;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_10x10_KHR = 0x93DB;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x10_KHR = 0x93DC;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_12x12_KHR = 0x93DD;
		//const uint GL_COMPRESSED_RGBA_ASTC_3x3x3_OES = 0x93C0;
		//const uint GL_COMPRESSED_RGBA_ASTC_4x3x3_OES = 0x93C1;
		//const uint GL_COMPRESSED_RGBA_ASTC_4x4x3_OES = 0x93C2;
		//const uint GL_COMPRESSED_RGBA_ASTC_4x4x4_OES = 0x93C3;
		//const uint GL_COMPRESSED_RGBA_ASTC_5x4x4_OES = 0x93C4;
		//const uint GL_COMPRESSED_RGBA_ASTC_5x5x4_OES = 0x93C5;
		//const uint GL_COMPRESSED_RGBA_ASTC_5x5x5_OES = 0x93C6;
		//const uint GL_COMPRESSED_RGBA_ASTC_6x5x5_OES = 0x93C7;
		//const uint GL_COMPRESSED_RGBA_ASTC_6x6x5_OES = 0x93C8;
		//const uint GL_COMPRESSED_RGBA_ASTC_6x6x6_OES = 0x93C9;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_3x3x3_OES = 0x93E0;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x3x3_OES = 0x93E1;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4x3_OES = 0x93E2;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_4x4x4_OES = 0x93E3;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x4x4_OES = 0x93E4;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5x4_OES = 0x93E5;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_5x5x5_OES = 0x93E6;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x5x5_OES = 0x93E7;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6x5_OES = 0x93E8;
		//const uint GL_COMPRESSED_SRGB8_ALPHA8_ASTC_6x6x6_OES = 0x93E9;
		//const uint GL_ATC_RGB_AMD = 0x8C92;
		//const uint GL_ATC_RGBA_EXPLICIT_ALPHA_AMD = 0x8C93;
		//const uint GL_ATC_RGBA_INTERPOLATED_ALPHA_AMD = 0x87EE;
		//const uint GL_PALETTE4_RGB8_OES = 0x8B90;
		//const uint GL_PALETTE4_RGBA8_OES = 0x8B91;
		//const uint GL_PALETTE4_R5_G6_B5_OES = 0x8B92;
		//const uint GL_PALETTE4_RGBA4_OES = 0x8B93;
		//const uint GL_PALETTE4_RGB5_A1_OES = 0x8B94;
		//const uint GL_PALETTE8_RGB8_OES = 0x8B95;
		//const uint GL_PALETTE8_RGBA8_OES = 0x8B96;
		//const uint GL_PALETTE8_R5_G6_B5_OES = 0x8B97;
		//const uint GL_PALETTE8_RGBA4_OES = 0x8B98;
		//const uint GL_PALETTE8_RGB5_A1_OES = 0x8B99;
		//const uint GL_DEPTH_COMPONENT16 = 0x81A5;
		//const uint GL_DEPTH_COMPONENT24 = 0x81A6;
		//const uint GL_DEPTH_COMPONENT32 = 0x81A7;
		//const uint GL_DEPTH_COMPONENT32F = 0x8CAC;
		//const uint GL_DEPTH_COMPONENT32F_NV = 0x8DAB;
		//const uint GL_STENCIL_INDEX1 = 0x8D46;
		//const uint GL_STENCIL_INDEX4 = 0x8D47;
		//const uint GL_STENCIL_INDEX8 = 0x8D48;
		//const uint GL_STENCIL_INDEX16 = 0x8D49;
		//const uint GL_DEPTH24_STENCIL8 = 0x88F0;
		//const uint GL_DEPTH32F_STENCIL8 = 0x8CAD;
		//const uint GL_DEPTH32F_STENCIL8_NV = 0x8DAC;

		//const uint GL_UNSIGNED_BYTE = 0x1401;
		//const uint GL_RED = 0x1903;
		//const uint GL_RG = 0x8227;
		//const uint GL_RGB = 0x1907;
		//const uint GL_BGR = 0x80E0;
		//const uint GL_RGBA = 0x1908;
		//const uint GL_BGRA = 0x80E1;
		//const uint GL_RED_INTEGER = 0x8D94;
		//const uint GL_RG_INTEGER = 0x8228;
		//const uint GL_RGB_INTEGER = 0x8D98;
		//const uint GL_BGR_INTEGER = 0x8D9A;
		//const uint GL_RGBA_INTEGER = 0x8D99;
		//const uint GL_BGRA_INTEGER = 0x8D9B;
		//const uint GL_STENCIL_INDEX = 0x1901;
		//const uint GL_DEPTH_COMPONENT = 0x1902;
		//const uint GL_DEPTH_STENCIL = 0x84F9;
		//const uint GL_BYTE = 0x1400;
		//const uint GL_UNSIGNED_SHORT = 0x1403;
		//const uint GL_SHORT = 0x1402;
		//const uint GL_HALF_FLOAT = 0x140B;
		//const uint GL_HALF_FLOAT_OES = 0x8D61;
		//const uint GL_UNSIGNED_INT = 0x1405;
		//const uint GL_INT = 0x1404;
		//const uint GL_FLOAT = 0x1406;
		//const uint GL_UNSIGNED_INT64 = 0x8BC2;
		//const uint GL_INT64 = 0x140E;
		//const uint GL_DOUBLE = 0x140A;
		//const uint GL_UNSIGNED_BYTE_3_3_2 = 0x8032;
		//const uint GL_UNSIGNED_BYTE_2_3_3_REV = 0x8362;
		//const uint GL_UNSIGNED_SHORT_5_6_5 = 0x8363;
		//const uint GL_UNSIGNED_SHORT_5_6_5_REV = 0x8364;
		//const uint GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
		//const uint GL_UNSIGNED_SHORT_4_4_4_4_REV = 0x8365;
		//const uint GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
		//const uint GL_UNSIGNED_SHORT_1_5_5_5_REV = 0x8366;
		//const uint GL_UNSIGNED_INT_8_8_8_8 = 0x8035;
		//const uint GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367;
		//const uint GL_UNSIGNED_INT_10_10_10_2 = 0x8036;
		//const uint GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368;
		//const uint GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B;
		//const uint GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3E;
		//const uint GL_UNSIGNED_INT_24_8 = 0x84FA;
		//const uint GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD;




		public static IEnumerable<PixelFormat> DepthFormats 
        {
            get
            {
				yield return PixelFormat.D32SFloatS8UInt;
				yield return PixelFormat.D32SFloat;
				yield return PixelFormat.D24UNormS8UInt;
				yield return PixelFormat.D16UNormS8UInt;
				yield return PixelFormat.D16UNorm;
			}
		}


        internal static IEnumerable<VkFormat> depth_formats
        {
            get
            {
                yield return VkFormat.D32SFloatS8UInt;
                yield return VkFormat.D32SFloat;
                yield return VkFormat.D24UNormS8UInt;
                yield return VkFormat.D16UNormS8UInt;
                yield return VkFormat.D16UNorm;
            }
        }


        internal static VkFormat ToVkFormat(this PixelFormat format)
		{
            switch (format)
            {
                case PixelFormat.Undefined:
                    break;
                case PixelFormat.R4G4UNormPack8:
                    break;
                case PixelFormat.R4G4B4A4UNormPack16:
                    break;
                case PixelFormat.B4G4R4A4UNormPack16:
                    break;
                case PixelFormat.R5G6B5UNormPack16:
                    break;
                case PixelFormat.B5G6R5UNormPack16:
                    break;
                case PixelFormat.R5G5B5A1UNormPack16:
                    break;
                case PixelFormat.B5G5R5A1UNormPack16:
                    break;
                case PixelFormat.A1R5G5B5UNormPack16:
                    break;
                case PixelFormat.R8UNorm:
                    break;
                case PixelFormat.R8SNorm:
                    break;
                case PixelFormat.R8UScaled:
                    break;
                case PixelFormat.R8SScaled:
                    break;
                case PixelFormat.R8UInt:
                    break;
                case PixelFormat.R8SInt:
                    break;
                case PixelFormat.R8SRgb:
                    break;
                case PixelFormat.R8G8UNorm:
                    break;
                case PixelFormat.R8G8SNorm:
                    break;
                case PixelFormat.R8G8UScaled:
                    break;
                case PixelFormat.R8G8SScaled:
                    break;
                case PixelFormat.R8G8UInt:
                    break;
                case PixelFormat.R8G8SInt:
                    break;
                case PixelFormat.R8G8SRgb:
                    break;
                case PixelFormat.R8G8B8UNorm:
                    break;
                case PixelFormat.R8G8B8SNorm:
                    break;
                case PixelFormat.R8G8B8UScaled:
                    break;
                case PixelFormat.R8G8B8SScaled:
                    break;
                case PixelFormat.R8G8B8UInt:
                    break;
                case PixelFormat.R8G8B8SInt:
                    break;
                case PixelFormat.R8G8B8SRgb:
                    break;
                case PixelFormat.B8G8R8UNorm:
                    break;
                case PixelFormat.B8G8R8SNorm:
                    break;
                case PixelFormat.B8G8R8UScaled:
                    break;
                case PixelFormat.B8G8R8SScaled:
                    break;
                case PixelFormat.B8G8R8UInt:
                    break;
                case PixelFormat.B8G8R8SInt:
                    break;
                case PixelFormat.B8G8R8SRgb:
                    break;
                case PixelFormat.R8G8B8A8UNorm:
                    break;
                case PixelFormat.R8G8B8A8SNorm:
                    break;
                case PixelFormat.R8G8B8A8UScaled:
                    break;
                case PixelFormat.R8G8B8A8SScaled:
                    break;
                case PixelFormat.R8G8B8A8UInt:
                    break;
                case PixelFormat.R8G8B8A8SInt:
                    break;
                case PixelFormat.R8G8B8A8SRgb:
                    break;
                case PixelFormat.B8G8R8A8UNorm:
                    break;
                case PixelFormat.B8G8R8A8SNorm:
                    break;
                case PixelFormat.B8G8R8A8UScaled:
                    break;
                case PixelFormat.B8G8R8A8SScaled:
                    break;
                case PixelFormat.B8G8R8A8UInt:
                    break;
                case PixelFormat.B8G8R8A8SInt:
                    break;
                case PixelFormat.B8G8R8A8SRgb:
                    break;
                case PixelFormat.A8B8G8R8UNormPack32:
                    break;
                case PixelFormat.A8B8G8R8SNormPack32:
                    break;
                case PixelFormat.A8B8G8R8UScaledPack32:
                    break;
                case PixelFormat.A8B8G8R8SScaledPack32:
                    break;
                case PixelFormat.A8B8G8R8UIntPack32:
                    break;
                case PixelFormat.A8B8G8R8SIntPack32:
                    break;
                case PixelFormat.A8B8G8R8SRgbPack32:
                    break;
                case PixelFormat.A2R10G10B10UNormPack32:
                    break;
                case PixelFormat.A2R10G10B10SNormPack32:
                    break;
                case PixelFormat.A2R10G10B10UScaledPack32:
                    break;
                case PixelFormat.A2R10G10B10SScaledPack32:
                    break;
                case PixelFormat.A2R10G10B10UIntPack32:
                    break;
                case PixelFormat.A2R10G10B10SIntPack32:
                    break;
                case PixelFormat.A2B10G10R10UNormPack32:
                    break;
                case PixelFormat.A2B10G10R10SNormPack32:
                    break;
                case PixelFormat.A2B10G10R10UScaledPack32:
                    break;
                case PixelFormat.A2B10G10R10SScaledPack32:
                    break;
                case PixelFormat.A2B10G10R10UIntPack32:
                    break;
                case PixelFormat.A2B10G10R10SIntPack32:
                    break;
                case PixelFormat.R16UNorm:
                    break;
                case PixelFormat.R16SNorm:
                    break;
                case PixelFormat.R16UScaled:
                    break;
                case PixelFormat.R16SScaled:
                    break;
                case PixelFormat.R16UInt:
                    break;
                case PixelFormat.R16SInt:
                    break;
                case PixelFormat.R16SFloat:
                    break;
                case PixelFormat.R16G16UNorm:
                    break;
                case PixelFormat.R16G16SNorm:
                    break;
                case PixelFormat.R16G16UScaled:
                    break;
                case PixelFormat.R16G16SScaled:
                    break;
                case PixelFormat.R16G16UInt:
                    break;
                case PixelFormat.R16G16SInt:
                    break;
                case PixelFormat.R16G16SFloat:
                    break;
                case PixelFormat.R16G16B16UNorm:
                    break;
                case PixelFormat.R16G16B16SNorm:
                    break;
                case PixelFormat.R16G16B16UScaled:
                    break;
                case PixelFormat.R16G16B16SScaled:
                    break;
                case PixelFormat.R16G16B16UInt:
                    break;
                case PixelFormat.R16G16B16SInt:
                    break;
                case PixelFormat.R16G16B16SFloat:
                    break;
                case PixelFormat.R16G16B16A16UNorm:
                    break;
                case PixelFormat.R16G16B16A16SNorm:
                    break;
                case PixelFormat.R16G16B16A16UScaled:
                    break;
                case PixelFormat.R16G16B16A16SScaled:
                    break;
                case PixelFormat.R16G16B16A16UInt:
                    break;
                case PixelFormat.R16G16B16A16SInt:
                    break;
                case PixelFormat.R16G16B16A16SFloat:
                    break;
                case PixelFormat.R32UInt:
                    break;
                case PixelFormat.R32SInt:
                    break;
                case PixelFormat.R32SFloat:
                    break;
                case PixelFormat.R32G32UInt:
                    break;
                case PixelFormat.R32G32SInt:
                    break;
                case PixelFormat.R32G32SFloat:
                    break;
                case PixelFormat.R32G32B32UInt:
                    break;
                case PixelFormat.R32G32B32SInt:
                    break;
                case PixelFormat.R32G32B32SFloat:
                    break;
                case PixelFormat.R32G32B32A32UInt:
                    break;
                case PixelFormat.R32G32B32A32SInt:
                    break;
                case PixelFormat.R32G32B32A32SFloat:
                    break;
                case PixelFormat.R64UInt:
                    break;
                case PixelFormat.R64SInt:
                    break;
                case PixelFormat.R64SFloat:
                    break;
                case PixelFormat.R64G64UInt:
                    break;
                case PixelFormat.R64G64SInt:
                    break;
                case PixelFormat.R64G64SFloat:
                    break;
                case PixelFormat.R64G64B64UInt:
                    break;
                case PixelFormat.R64G64B64SInt:
                    break;
                case PixelFormat.R64G64B64SFloat:
                    break;
                case PixelFormat.R64G64B64A64UInt:
                    break;
                case PixelFormat.R64G64B64A64SInt:
                    break;
                case PixelFormat.R64G64B64A64SFloat:
                    break;
                case PixelFormat.B10G11R11UFloatPack32:
                    break;
                case PixelFormat.E5B9G9R9UFloatPack32:
                    break;
                case PixelFormat.D16UNorm:
                    break;
                case PixelFormat.X8D24UNormPack32:
                    break;
                case PixelFormat.D32SFloat:
                    break;
                case PixelFormat.S8UInt:
                    break;
                case PixelFormat.D16UNormS8UInt:
                    break;
                case PixelFormat.D24UNormS8UInt:
                    break;
                case PixelFormat.D32SFloatS8UInt:
                    break;
                case PixelFormat.BC1RGBUNormBlock:
                    break;
                case PixelFormat.BC1RGBSRgbBlock:
                    break;
                case PixelFormat.BC1RGBAUNormBlock:
                    break;
                case PixelFormat.BC1RGBASRgbBlock:
                    break;
                case PixelFormat.BC2UNormBlock:
                    break;
                case PixelFormat.BC2SRgbBlock:
                    break;
                case PixelFormat.BC3UNormBlock:
                    break;
                case PixelFormat.BC3SRgbBlock:
                    break;
                case PixelFormat.BC4UNormBlock:
                    break;
                case PixelFormat.BC4SNormBlock:
                    break;
                case PixelFormat.BC5UNormBlock:
                    break;
                case PixelFormat.BC5SNormBlock:
                    break;
                case PixelFormat.BC6HUFloatBlock:
                    break;
                case PixelFormat.BC6HSFloatBlock:
                    break;
                case PixelFormat.BC7UNormBlock:
                    break;
                case PixelFormat.BC7SRgbBlock:
                    break;
                case PixelFormat.ETC2R8G8B8UNormBlock:
                    break;
                case PixelFormat.ETC2R8G8B8SRgbBlock:
                    break;
                case PixelFormat.ETC2R8G8B8A1UNormBlock:
                    break;
                case PixelFormat.ETC2R8G8B8A1SRgbBlock:
                    break;
                case PixelFormat.ETC2R8G8B8A8UNormBlock:
                    break;
                case PixelFormat.ETC2R8G8B8A8SRgbBlock:
                    break;
                case PixelFormat.EACR11UNormBlock:
                    break;
                case PixelFormat.EACR11SNormBlock:
                    break;
                case PixelFormat.EACR11G11UNormBlock:
                    break;
                case PixelFormat.EACR11G11SNormBlock:
                    break;
                case PixelFormat.ASTC4x4UNormBlock:
                    break;
                case PixelFormat.ASTC4x4SRgbBlock:
                    break;
                case PixelFormat.ASTC5x4UNormBlock:
                    break;
                case PixelFormat.ASTC5x4SRgbBlock:
                    break;
                case PixelFormat.ASTC5x5UNormBlock:
                    break;
                case PixelFormat.ASTC5x5SRgbBlock:
                    break;
                case PixelFormat.ASTC6x5UNormBlock:
                    break;
                case PixelFormat.ASTC6x5SRgbBlock:
                    break;
                case PixelFormat.ASTC6x6UNormBlock:
                    break;
                case PixelFormat.ASTC6x6SRgbBlock:
                    break;
                case PixelFormat.ASTC8x5UNormBlock:
                    break;
                case PixelFormat.ASTC8x5SRgbBlock:
                    break;
                case PixelFormat.ASTC8x6UNormBlock:
                    break;
                case PixelFormat.ASTC8x6SRgbBlock:
                    break;
                case PixelFormat.ASTC8x8UNormBlock:
                    break;
                case PixelFormat.ASTC8x8SRgbBlock:
                    break;
                case PixelFormat.ASTC10x5UNormBlock:
                    break;
                case PixelFormat.ASTC10x5SRgbBlock:
                    break;
                case PixelFormat.ASTC10x6UNormBlock:
                    break;
                case PixelFormat.ASTC10x6SRgbBlock:
                    break;
                case PixelFormat.ASTC10x8UNormBlock:
                    break;
                case PixelFormat.ASTC10x8SRgbBlock:
                    break;
                case PixelFormat.ASTC10x10UNormBlock:
                    break;
                case PixelFormat.ASTC10x10SRgbBlock:
                    break;
                case PixelFormat.ASTC12x10UNormBlock:
                    break;
                case PixelFormat.ASTC12x10SRgbBlock:
                    break;
                case PixelFormat.ASTC12x12UNormBlock:
                    break;
                case PixelFormat.ASTC12x12SRgbBlock:
                    break;
                case PixelFormat.G8B8G8R8422UNorm:
                    break;
                case PixelFormat.B8G8R8G8422UNorm:
                    break;
                case PixelFormat.G8B8R83Plane420UNorm:
                    break;
                case PixelFormat.G8B8R82Plane420UNorm:
                    break;
                case PixelFormat.G8B8R83Plane422UNorm:
                    break;
                case PixelFormat.G8B8R82Plane422UNorm:
                    break;
                case PixelFormat.G8B8R83Plane444UNorm:
                    break;
                case PixelFormat.R10X6UNormPack16:
                    break;
                case PixelFormat.R10X6G10X6UNorm2Pack16:
                    break;
                case PixelFormat.R10X6G10X6B10X6A10X6UNorm4Pack16:
                    break;
                case PixelFormat.G10X6B10X6G10X6R10X6422UNorm4Pack16:
                    break;
                case PixelFormat.B10X6G10X6R10X6G10X6422UNorm4Pack16:
                    break;
                case PixelFormat.G10X6B10X6R10X63Plane420UNorm3Pack16:
                    break;
                case PixelFormat.G10X6B10X6R10X62Plane420UNorm3Pack16:
                    break;
                case PixelFormat.G10X6B10X6R10X63Plane422UNorm3Pack16:
                    break;
                case PixelFormat.G10X6B10X6R10X62Plane422UNorm3Pack16:
                    break;
                case PixelFormat.G10X6B10X6R10X63Plane444UNorm3Pack16:
                    break;
                case PixelFormat.R12X4UNormPack16:
                    break;
                case PixelFormat.R12X4G12X4UNorm2Pack16:
                    break;
                case PixelFormat.R12X4G12X4B12X4A12X4UNorm4Pack16:
                    break;
                case PixelFormat.G12X4B12X4G12X4R12X4422UNorm4Pack16:
                    break;
                case PixelFormat.B12X4G12X4R12X4G12X4422UNorm4Pack16:
                    break;
                case PixelFormat.G12X4B12X4R12X43Plane420UNorm3Pack16:
                    break;
                case PixelFormat.G12X4B12X4R12X42Plane420UNorm3Pack16:
                    break;
                case PixelFormat.G12X4B12X4R12X43Plane422UNorm3Pack16:
                    break;
                case PixelFormat.G12X4B12X4R12X42Plane422UNorm3Pack16:
                    break;
                case PixelFormat.G12X4B12X4R12X43Plane444UNorm3Pack16:
                    break;
                case PixelFormat.G16B16G16R16422UNorm:
                    break;
                case PixelFormat.B16G16R16G16422UNorm:
                    break;
                case PixelFormat.G16B16R163Plane420UNorm:
                    break;
                case PixelFormat.G16B16R162Plane420UNorm:
                    break;
                case PixelFormat.G16B16R163Plane422UNorm:
                    break;
                case PixelFormat.G16B16R162Plane422UNorm:
                    break;
                case PixelFormat.G16B16R163Plane444UNorm:
                    break;
                case PixelFormat.PVRTC12BPPUNormBlockImg:
                    break;
                case PixelFormat.PVRTC14BPPUNormBlockImg:
                    break;
                case PixelFormat.PVRTC22BPPUNormBlockImg:
                    break;
                case PixelFormat.PVRTC24BPPUNormBlockImg:
                    break;
                case PixelFormat.PVRTC12BPPSRgbBlockImg:
                    break;
                case PixelFormat.PVRTC14BPPSRgbBlockImg:
                    break;
                case PixelFormat.PVRTC22BPPSRgbBlockImg:
                    break;
                case PixelFormat.PVRTC24BPPSRgbBlockImg:
                    break;
                case PixelFormat.ASTC4x4SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC5x4SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC5x5SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC6x5SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC6x6SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC8x5SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC8x6SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC8x8SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC10x5SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC10x6SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC10x8SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC10x10SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC12x10SFloatBlockEXT:
                    break;
                case PixelFormat.ASTC12x12SFloatBlockEXT:
                    break;
                //case PixelFormat.G8B8G8R8422UNormKHR:
                //    break;
                //case PixelFormat.B8G8R8G8422UNormKHR:
                //    break;
                //case PixelFormat.G8B8R83Plane420UNormKHR:
                //    break;
                //case PixelFormat.G8B8R82Plane420UNormKHR:
                //    break;
                //case PixelFormat.G8B8R83Plane422UNormKHR:
                //    break;
                //case PixelFormat.G8B8R82Plane422UNormKHR:
                //    break;
                //case PixelFormat.G8B8R83Plane444UNormKHR:
                //    break;
                //case PixelFormat.R10X6UNormPack16KHR:
                //    break;
                //case PixelFormat.R10X6G10X6UNorm2Pack16KHR:
                //    break;
                //case PixelFormat.R10X6G10X6B10X6A10X6UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6G10X6R10X6422UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.B10X6G10X6R10X6G10X6422UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6R10X63Plane420UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6R10X62Plane420UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6R10X63Plane422UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6R10X62Plane422UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G10X6B10X6R10X63Plane444UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.R12X4UNormPack16KHR:
                //    break;
                //case PixelFormat.R12X4G12X4UNorm2Pack16KHR:
                //    break;
                //case PixelFormat.R12X4G12X4B12X4A12X4UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4G12X4R12X4422UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.B12X4G12X4R12X4G12X4422UNorm4Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4R12X43Plane420UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4R12X42Plane420UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4R12X43Plane422UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4R12X42Plane422UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G12X4B12X4R12X43Plane444UNorm3Pack16KHR:
                //    break;
                //case PixelFormat.G16B16G16R16422UNormKHR:
                //    break;
                //case PixelFormat.B16G16R16G16422UNormKHR:
                //    break;
                //case PixelFormat.G16B16R163Plane420UNormKHR:
                //    break;
                //case PixelFormat.G16B16R162Plane420UNormKHR:
                //    break;
                //case PixelFormat.G16B16R163Plane422UNormKHR:
                //    break;
                //case PixelFormat.G16B16R162Plane422UNormKHR:
                //    break;
                //case PixelFormat.G16B16R163Plane444UNormKHR:
                //    break;
                default:
                    break;
            }
            return VkFormat.Undefined;
        }


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


        internal static bool is_srgb(this VkFormat format)
        {
            switch (format)
            {
                case VkFormat.A8B8G8R8SRgbPack32:
                case VkFormat.R8G8B8A8SRgb:
                case VkFormat.B8G8R8A8SRgb:
                case VkFormat.R8SRgb:
                case VkFormat.R8G8SRgb:
                case VkFormat.R8G8B8SRgb:
                case VkFormat.B8G8R8SRgb:
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
