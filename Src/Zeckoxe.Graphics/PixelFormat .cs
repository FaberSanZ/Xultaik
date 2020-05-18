﻿// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PixelFormat.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    /// <summary>
    /// Available pixel formats.
    /// </summary>
    public enum PixelFormat
    {
		Undefined = 0,
		R4G4UNormPack8 = 1,
		R4G4B4A4UNormPack16 = 2,
		B4G4R4A4UNormPack16 = 3,
		R5G6B5UNormPack16 = 4,
		B5G6R5UNormPack16 = 5,
		R5G5B5A1UNormPack16 = 6,
		B5G5R5A1UNormPack16 = 7,
		A1R5G5B5UNormPack16 = 8,
		R8UNorm = 9,
		R8SNorm = 10,
		R8UScaled = 11,
		R8SScaled = 12,
		R8UInt = 13,
		R8SInt = 14,
		R8SRgb = 15,
		R8G8UNorm = 16,
		R8G8SNorm = 17,
		R8G8UScaled = 18,
		R8G8SScaled = 19,
		R8G8UInt = 20,
		R8G8SInt = 21,
		R8G8SRgb = 22,
		R8G8B8UNorm = 23,
		R8G8B8SNorm = 24,
		R8G8B8UScaled = 25,
		R8G8B8SScaled = 26,
		R8G8B8UInt = 27,
		R8G8B8SInt = 28,
		R8G8B8SRgb = 29,
		B8G8R8UNorm = 30,
		B8G8R8SNorm = 31,
		B8G8R8UScaled = 32,
		B8G8R8SScaled = 33,
		B8G8R8UInt = 34,
		B8G8R8SInt = 35,
		B8G8R8SRgb = 36,
		R8G8B8A8UNorm = 37,
		R8G8B8A8SNorm = 38,
		R8G8B8A8UScaled = 39,
		R8G8B8A8SScaled = 40,
		R8G8B8A8UInt = 41,
		R8G8B8A8SInt = 42,
		R8G8B8A8SRgb = 43,
		B8G8R8A8UNorm = 44,
		B8G8R8A8SNorm = 45,
		B8G8R8A8UScaled = 46,
		B8G8R8A8SScaled = 47,
		B8G8R8A8UInt = 48,
		B8G8R8A8SInt = 49,
		B8G8R8A8SRgb = 50,
		A8B8G8R8UNormPack32 = 51,
		A8B8G8R8SNormPack32 = 52,
		A8B8G8R8UScaledPack32 = 53,
		A8B8G8R8SScaledPack32 = 54,
		A8B8G8R8UIntPack32 = 55,
		A8B8G8R8SIntPack32 = 56,
		A8B8G8R8SRgbPack32 = 57,
		A2R10G10B10UNormPack32 = 58,
		A2R10G10B10SNormPack32 = 59,
		A2R10G10B10UScaledPack32 = 60,
		A2R10G10B10SScaledPack32 = 61,
		A2R10G10B10UIntPack32 = 62,
		A2R10G10B10SIntPack32 = 63,
		A2B10G10R10UNormPack32 = 64,
		A2B10G10R10SNormPack32 = 65,
		A2B10G10R10UScaledPack32 = 66,
		A2B10G10R10SScaledPack32 = 67,
		A2B10G10R10UIntPack32 = 68,
		A2B10G10R10SIntPack32 = 69,
		R16UNorm = 70,
		R16SNorm = 71,
		R16UScaled = 72,
		R16SScaled = 73,
		R16UInt = 74,
		R16SInt = 75,
		R16SFloat = 76,
		R16G16UNorm = 77,
		R16G16SNorm = 78,
		R16G16UScaled = 79,
		R16G16SScaled = 80,
		R16G16UInt = 81,
		R16G16SInt = 82,
		R16G16SFloat = 83,
		R16G16B16UNorm = 84,
		R16G16B16SNorm = 85,
		R16G16B16UScaled = 86,
		R16G16B16SScaled = 87,
		R16G16B16UInt = 88,
		R16G16B16SInt = 89,
		R16G16B16SFloat = 90,
		R16G16B16A16UNorm = 91,
		R16G16B16A16SNorm = 92,
		R16G16B16A16UScaled = 93,
		R16G16B16A16SScaled = 94,
		R16G16B16A16UInt = 95,
		R16G16B16A16SInt = 96,
		R16G16B16A16SFloat = 97,
		R32UInt = 98,
		R32SInt = 99,
		R32SFloat = 100,
		R32G32UInt = 101,
		R32G32SInt = 102,
		R32G32SFloat = 103,
		R32G32B32UInt = 104,
		R32G32B32SInt = 105,
		R32G32B32SFloat = 106,
		R32G32B32A32UInt = 107,
		R32G32B32A32SInt = 108,
		R32G32B32A32SFloat = 109,
		R64UInt = 110,
		R64SInt = 111,
		R64SFloat = 112,
		R64G64UInt = 113,
		R64G64SInt = 114,
		R64G64SFloat = 115,
		R64G64B64UInt = 116,
		R64G64B64SInt = 117,
		R64G64B64SFloat = 118,
		R64G64B64A64UInt = 119,
		R64G64B64A64SInt = 120,
		R64G64B64A64SFloat = 121,
		B10G11R11UFloatPack32 = 122,
		E5B9G9R9UFloatPack32 = 123,
		D16UNorm = 124,
		X8D24UNormPack32 = 125,
		D32SFloat = 126,
		S8UInt = 127,
		D16UNormS8UInt = 128,
		D24UNormS8UInt = 129,
		D32SFloatS8UInt = 130,
		BC1RGBUNormBlock = 131,
		BC1RGBSRgbBlock = 132,
		BC1RGBAUNormBlock = 133,
		BC1RGBASRgbBlock = 134,
		BC2UNormBlock = 135,
		BC2SRgbBlock = 136,
		BC3UNormBlock = 137,
		BC3SRgbBlock = 138,
		BC4UNormBlock = 139,
		BC4SNormBlock = 140,
		BC5UNormBlock = 141,
		BC5SNormBlock = 142,
		BC6HUFloatBlock = 143,
		BC6HSFloatBlock = 144,
		BC7UNormBlock = 145,
		BC7SRgbBlock = 146,
		ETC2R8G8B8UNormBlock = 147,
		ETC2R8G8B8SRgbBlock = 148,
		ETC2R8G8B8A1UNormBlock = 149,
		ETC2R8G8B8A1SRgbBlock = 150,
		ETC2R8G8B8A8UNormBlock = 151,
		ETC2R8G8B8A8SRgbBlock = 152,
		EACR11UNormBlock = 153,
		EACR11SNormBlock = 154,
		EACR11G11UNormBlock = 155,
		EACR11G11SNormBlock = 156,
		ASTC4x4UNormBlock = 157,
		ASTC4x4SRgbBlock = 158,
		ASTC5x4UNormBlock = 159,
		ASTC5x4SRgbBlock = 160,
		ASTC5x5UNormBlock = 161,
		ASTC5x5SRgbBlock = 162,
		ASTC6x5UNormBlock = 163,
		ASTC6x5SRgbBlock = 164,
		ASTC6x6UNormBlock = 165,
		ASTC6x6SRgbBlock = 166,
		ASTC8x5UNormBlock = 167,
		ASTC8x5SRgbBlock = 168,
		ASTC8x6UNormBlock = 169,
		ASTC8x6SRgbBlock = 170,
		ASTC8x8UNormBlock = 171,
		ASTC8x8SRgbBlock = 172,
		ASTC10x5UNormBlock = 173,
		ASTC10x5SRgbBlock = 174,
		ASTC10x6UNormBlock = 175,
		ASTC10x6SRgbBlock = 176,
		ASTC10x8UNormBlock = 177,
		ASTC10x8SRgbBlock = 178,
		ASTC10x10UNormBlock = 179,
		ASTC10x10SRgbBlock = 180,
		ASTC12x10UNormBlock = 181,
		ASTC12x10SRgbBlock = 182,
		ASTC12x12UNormBlock = 183,
		ASTC12x12SRgbBlock = 184,
		G8B8G8R8422UNorm = 1000156000,
		B8G8R8G8422UNorm = 1000156001,
		G8B8R83Plane420UNorm = 1000156002,
		G8B8R82Plane420UNorm = 1000156003,
		G8B8R83Plane422UNorm = 1000156004,
		G8B8R82Plane422UNorm = 1000156005,
		G8B8R83Plane444UNorm = 1000156006,
		R10X6UNormPack16 = 1000156007,
		R10X6G10X6UNorm2Pack16 = 1000156008,
		R10X6G10X6B10X6A10X6UNorm4Pack16 = 1000156009,
		G10X6B10X6G10X6R10X6422UNorm4Pack16 = 1000156010,
		B10X6G10X6R10X6G10X6422UNorm4Pack16 = 1000156011,
		G10X6B10X6R10X63Plane420UNorm3Pack16 = 1000156012,
		G10X6B10X6R10X62Plane420UNorm3Pack16 = 1000156013,
		G10X6B10X6R10X63Plane422UNorm3Pack16 = 1000156014,
		G10X6B10X6R10X62Plane422UNorm3Pack16 = 1000156015,
		G10X6B10X6R10X63Plane444UNorm3Pack16 = 1000156016,
		R12X4UNormPack16 = 1000156017,
		R12X4G12X4UNorm2Pack16 = 1000156018,
		R12X4G12X4B12X4A12X4UNorm4Pack16 = 1000156019,
		G12X4B12X4G12X4R12X4422UNorm4Pack16 = 1000156020,
		B12X4G12X4R12X4G12X4422UNorm4Pack16 = 1000156021,
		G12X4B12X4R12X43Plane420UNorm3Pack16 = 1000156022,
		G12X4B12X4R12X42Plane420UNorm3Pack16 = 1000156023,
		G12X4B12X4R12X43Plane422UNorm3Pack16 = 1000156024,
		G12X4B12X4R12X42Plane422UNorm3Pack16 = 1000156025,
		G12X4B12X4R12X43Plane444UNorm3Pack16 = 1000156026,
		G16B16G16R16422UNorm = 1000156027,
		B16G16R16G16422UNorm = 1000156028,
		G16B16R163Plane420UNorm = 1000156029,
		G16B16R162Plane420UNorm = 1000156030,
		G16B16R163Plane422UNorm = 1000156031,
		G16B16R162Plane422UNorm = 1000156032,
		G16B16R163Plane444UNorm = 1000156033,
		PVRTC12BPPUNormBlockImg = 1000054000,
		PVRTC14BPPUNormBlockImg = 1000054001,
		PVRTC22BPPUNormBlockImg = 1000054002,
		PVRTC24BPPUNormBlockImg = 1000054003,
		PVRTC12BPPSRgbBlockImg = 1000054004,
		PVRTC14BPPSRgbBlockImg = 1000054005,
		PVRTC22BPPSRgbBlockImg = 1000054006,
		PVRTC24BPPSRgbBlockImg = 1000054007,
		ASTC4x4SFloatBlockEXT = 1000066000,
		ASTC5x4SFloatBlockEXT = 1000066001,
		ASTC5x5SFloatBlockEXT = 1000066002,
		ASTC6x5SFloatBlockEXT = 1000066003,
		ASTC6x6SFloatBlockEXT = 1000066004,
		ASTC8x5SFloatBlockEXT = 1000066005,
		ASTC8x6SFloatBlockEXT = 1000066006,
		ASTC8x8SFloatBlockEXT = 1000066007,
		ASTC10x5SFloatBlockEXT = 1000066008,
		ASTC10x6SFloatBlockEXT = 1000066009,
		ASTC10x8SFloatBlockEXT = 1000066010,
		ASTC10x10SFloatBlockEXT = 1000066011,
		ASTC12x10SFloatBlockEXT = 1000066012,
		ASTC12x12SFloatBlockEXT = 1000066013,
		G8B8G8R8422UNormKHR = G8B8G8R8422UNorm,
		B8G8R8G8422UNormKHR = B8G8R8G8422UNorm,
		G8B8R83Plane420UNormKHR = G8B8R83Plane420UNorm,
		G8B8R82Plane420UNormKHR = G8B8R82Plane420UNorm,
		G8B8R83Plane422UNormKHR = G8B8R83Plane422UNorm,
		G8B8R82Plane422UNormKHR = G8B8R82Plane422UNorm,
		G8B8R83Plane444UNormKHR = G8B8R83Plane444UNorm,
		R10X6UNormPack16KHR = R10X6UNormPack16,
		R10X6G10X6UNorm2Pack16KHR = R10X6G10X6UNorm2Pack16,
		R10X6G10X6B10X6A10X6UNorm4Pack16KHR = R10X6G10X6B10X6A10X6UNorm4Pack16,
		G10X6B10X6G10X6R10X6422UNorm4Pack16KHR = G10X6B10X6G10X6R10X6422UNorm4Pack16,
		B10X6G10X6R10X6G10X6422UNorm4Pack16KHR = B10X6G10X6R10X6G10X6422UNorm4Pack16,
		G10X6B10X6R10X63Plane420UNorm3Pack16KHR = G10X6B10X6R10X63Plane420UNorm3Pack16,
		G10X6B10X6R10X62Plane420UNorm3Pack16KHR = G10X6B10X6R10X62Plane420UNorm3Pack16,
		G10X6B10X6R10X63Plane422UNorm3Pack16KHR = G10X6B10X6R10X63Plane422UNorm3Pack16,
		G10X6B10X6R10X62Plane422UNorm3Pack16KHR = G10X6B10X6R10X62Plane422UNorm3Pack16,
		G10X6B10X6R10X63Plane444UNorm3Pack16KHR = G10X6B10X6R10X63Plane444UNorm3Pack16,
		R12X4UNormPack16KHR = R12X4UNormPack16,
		R12X4G12X4UNorm2Pack16KHR = R12X4G12X4UNorm2Pack16,
		R12X4G12X4B12X4A12X4UNorm4Pack16KHR = R12X4G12X4B12X4A12X4UNorm4Pack16,
		G12X4B12X4G12X4R12X4422UNorm4Pack16KHR = G12X4B12X4G12X4R12X4422UNorm4Pack16,
		B12X4G12X4R12X4G12X4422UNorm4Pack16KHR = B12X4G12X4R12X4G12X4422UNorm4Pack16,
		G12X4B12X4R12X43Plane420UNorm3Pack16KHR = G12X4B12X4R12X43Plane420UNorm3Pack16,
		G12X4B12X4R12X42Plane420UNorm3Pack16KHR = G12X4B12X4R12X42Plane420UNorm3Pack16,
		G12X4B12X4R12X43Plane422UNorm3Pack16KHR = G12X4B12X4R12X43Plane422UNorm3Pack16,
		G12X4B12X4R12X42Plane422UNorm3Pack16KHR = G12X4B12X4R12X42Plane422UNorm3Pack16,
		G12X4B12X4R12X43Plane444UNorm3Pack16KHR = G12X4B12X4R12X43Plane444UNorm3Pack16,
		G16B16G16R16422UNormKHR = G16B16G16R16422UNorm,
		B16G16R16G16422UNormKHR = B16G16R16G16422UNorm,
		G16B16R163Plane420UNormKHR = G16B16R163Plane420UNorm,
		G16B16R162Plane420UNormKHR = G16B16R162Plane420UNorm,
		G16B16R163Plane422UNormKHR = G16B16R163Plane422UNorm,
		G16B16R162Plane422UNormKHR = G16B16R162Plane422UNorm,
		G16B16R163Plane444UNormKHR = G16B16R163Plane444UNorm,
	}

}
