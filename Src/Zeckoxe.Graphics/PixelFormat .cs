// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

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
    public enum PixelFormat : int
    {
        /// <summary>
        /// Indicates that the format is not specified.
        /// </summary>
        Undefined = unchecked(0),


        /// <summary>
        /// Specifies a two-component, 8-bit packed unsigned normalized format that has a 4-bit R
        /// component in bits 4..7, and a 4-bit G component in bits 0..3.
        /// </summary>
        R4G4_UNorm_Pack8 = unchecked(1),


        /// <summary>
        /// Specifies a four-component, 16-bit packed unsigned normalized format that has a 4-bit R
        /// component in bits 12..15, a 4-bit G component in bits 8..11, a 4-bit B component in bits
        /// 4..7, and a 4-bit A component in bits 0..3.
        /// </summary>
        R4G4B4A4_UNorm_Pack16 = unchecked(2),


        /// <summary>
        /// Specifies a four-component, 16-bit packed unsigned normalized format that has a 4-bit B
        /// component in bits 12..15, a 4-bit G component in bits 8..11, a 4-bit R component in bits
        /// 4..7, and a 4-bit A component in bits 0..3.
        /// </summary>
        B4G4R4A4_UNorm_Pack16 = unchecked(3),


        /// <summary>
        /// Specifies a three-component, 16-bit packed unsigned normalized format that has a 5-bit R
        /// component in bits 11..15, a 6-bit G component in bits 5..10, and a 5-bit B component in
        /// bits 0..4.
        /// </summary>
        R5G6B5_UNorm_Pack16 = unchecked(4),

        B5G6R5_UNorm_Pack16 = unchecked(5),

        R5G5b5A1_UNorm_Pack16 = unchecked(6),

        B5G5R5A1_UNorm_Pack16 = unchecked(7),

        A1R5G5B5_UNorm_Pack16 = unchecked(8),

        R8_UNorm = unchecked(9),

        R8_SNorm = unchecked(10),

        R8_UScaled = unchecked(11),

        R8_SScaled = unchecked(12),

        R8_Uint = unchecked(13),

        R8_SInt = unchecked(14),

        R8_SRGB = unchecked(15),

        R8G8_UNorm = unchecked(16),

        R8G8_SNorm = unchecked(17),

        R8G8_UScaled = unchecked(18),

        R8G8_SScaled = unchecked(19),

        R8G8_UInt = unchecked(20),

        R8G8_SInt = unchecked(21),

        R8G8_SRGB = unchecked(22),

        R8G8B8_UNorm = unchecked(23),

        R8G8B8_SNorm = unchecked(24),
        R8g8b8Uscaled = 25,
        R8g8b8Sscaled = 26,
        R8g8b8Uint = 27,
        R8g8b8Sint = 28,
        R8g8b8Srgb = 29,
        B8g8r8Unorm = 30,
        B8g8r8Snorm = 31,
        B8g8r8Uscaled = 32,
        B8g8r8Sscaled = 33,
        B8g8r8Uint = 34,
        B8g8r8Sint = 35,
        B8g8r8Srgb = 36,
        R8g8b8a8Unorm = 37,
        R8g8b8a8Snorm = 38,
        R8g8b8a8Uscaled = 39,
        R8g8b8a8Sscaled = 40,
        R8g8b8a8Uint = 41,
        R8g8b8a8Sint = 42,
        R8g8b8a8Srgb = 43,
        B8g8r8a8Unorm = 44,
        B8g8r8a8Snorm = 45,
        B8g8r8a8Uscaled = 46,
        B8g8r8a8Sscaled = 47,
        B8g8r8a8Uint = 48,
        B8g8r8a8Sint = 49,
        B8g8r8a8Srgb = 50,
        A8b8g8r8UnormPack32 = 51,
        A8b8g8r8SnormPack32 = 52,
        A8b8g8r8UscaledPack32 = 53,
        A8b8g8r8SscaledPack32 = 54,
        A8b8g8r8UintPack32 = 55,
        A8b8g8r8SintPack32 = 56,
        A8b8g8r8SrgbPack32 = 57,
        A2r10g10b10UnormPack32 = 58,
        A2r10g10b10SnormPack32 = 59,
        A2r10g10b10UscaledPack32 = 60,
        A2r10g10b10SscaledPack32 = 61,
        A2r10g10b10UintPack32 = 62,
        A2r10g10b10SintPack32 = 63,
        A2b10g10r10UnormPack32 = 64,
        A2b10g10r10SnormPack32 = 65,
        A2b10g10r10UscaledPack32 = 66,
        A2b10g10r10SscaledPack32 = 67,
        A2b10g10r10UintPack32 = 68,
        A2b10g10r10SintPack32 = 69,
        R16Unorm = 70,
        R16Snorm = 71,
        R16Uscaled = 72,
        R16Sscaled = 73,
        R16Uint = 74,
        R16Sint = 75,
        R16Sfloat = 76,
        R16g16Unorm = 77,
        R16g16Snorm = 78,
        R16g16Uscaled = 79,
        R16g16Sscaled = 80,
        R16g16Uint = 81,
        R16g16Sint = 82,
        R16g16Sfloat = 83,
        R16g16b16Unorm = 84,
        R16g16b16Snorm = 85,
        R16g16b16Uscaled = 86,
        R16g16b16Sscaled = 87,
        R16g16b16Uint = 88,
        R16g16b16Sint = 89,
        R16g16b16Sfloat = 90,
        R16g16b16a16Unorm = 91,
        R16g16b16a16Snorm = 92,
        R16g16b16a16Uscaled = 93,
        R16g16b16a16Sscaled = 94,
        R16g16b16a16Uint = 95,
        R16g16b16a16Sint = 96,
        R16g16b16a16Sfloat = 97,
        R32Uint = 98,
        R32Sint = 99,
        R32Sfloat = 100,
        R32g32Uint = 101,
        R32g32Sint = 102,
        R32g32Sfloat = 103,
        R32g32b32Uint = 104,
        R32g32b32Sint = 105,
        R32g32b32Sfloat = 106,
        R32g32b32a32Uint = 107,
        R32g32b32a32Sint = 108,
        R32g32b32a32Sfloat = 109,
        R64Uint = 110,
        R64Sint = 111,
        R64Sfloat = 112,
        R64g64Uint = 113,
        R64g64Sint = 114,
        R64g64Sfloat = 115,
        R64g64b64Uint = 116,
        R64g64b64Sint = 117,
        R64g64b64Sfloat = 118,
        R64g64b64a64Uint = 119,
        R64g64b64a64Sint = 120,
        R64g64b64a64Sfloat = 121,
        B10g11r11UfloatPack32 = 122,
        E5b9g9r9UfloatPack32 = 123,
        D16Unorm = 124,
        X8D24UnormPack32 = 125,
        D32Sfloat = 126,
        S8Uint = 127,
        D16UnormS8Uint = 128,
        D24UnormS8Uint = 129,
        D32SfloatS8Uint = 130,
        Bc1RgbUnormBlock = 131,
        Bc1RgbSrgbBlock = 132,
        Bc1RgbaUnormBlock = 133,
        Bc1RgbaSrgbBlock = 134,
        Bc2UnormBlock = 135,
        Bc2SrgbBlock = 136,
        Bc3UnormBlock = 137,
        Bc3SrgbBlock = 138,
        Bc4UnormBlock = 139,
        Bc4SnormBlock = 140,
        Bc5UnormBlock = 141,
        Bc5SnormBlock = 142,
        Bc6hUfloatBlock = 143,
        Bc6hSfloatBlock = 144,
        Bc7UnormBlock = 145,
        Bc7SrgbBlock = 146,
        Etc2R8g8b8UnormBlock = 147,
        Etc2R8g8b8SrgbBlock = 148,
        Etc2R8g8b8a1UnormBlock = 149,
        Etc2R8g8b8a1SrgbBlock = 150,
        Etc2R8g8b8a8UnormBlock = 151,
        Etc2R8g8b8a8SrgbBlock = 152,
        EacR11UnormBlock = 153,
        EacR11SnormBlock = 154,
        EacR11g11UnormBlock = 155,
        EacR11g11SnormBlock = 156,
        Astc4x4UnormBlock = 157,
        Astc4x4SrgbBlock = 158,
        Astc5x4UnormBlock = 159,
        Astc5x4SrgbBlock = 160,
        Astc5x5UnormBlock = 161,
        Astc5x5SrgbBlock = 162,
        Astc6x5UnormBlock = 163,
        Astc6x5SrgbBlock = 164,
        Astc6x6UnormBlock = 165,
        Astc6x6SrgbBlock = 166,
        Astc8x5UnormBlock = 167,
        Astc8x5SrgbBlock = 168,
        Astc8x6UnormBlock = 169,
        Astc8x6SrgbBlock = 170,
        Astc8x8UnormBlock = 171,
        Astc8x8SrgbBlock = 172,
        Astc10x5UnormBlock = 173,
        Astc10x5SrgbBlock = 174,
        Astc10x6UnormBlock = 175,
        Astc10x6SrgbBlock = 176,
        Astc10x8UnormBlock = 177,
        Astc10x8SrgbBlock = 178,
        Astc10x10UnormBlock = 179,
        Astc10x10SrgbBlock = 180,
        Astc12x10UnormBlock = 181,
        Astc12x10SrgbBlock = 182,
        Astc12x12UnormBlock = 183,
        Astc12x12SrgbBlock = 184,
        Pvrtc12bppUnormBlockImg = 185,
        Pvrtc14bppUnormBlockImg = 186,
        Pvrtc22bppUnormBlockImg = 187,
        Pvrtc24bppUnormBlockImg = 188,
        Pvrtc12bppSrgbBlockImg = 189,
        Pvrtc14bppSrgbBlockImg = 1000054005,
        Pvrtc22bppSrgbBlockImg = 1000054006,
        Pvrtc24bppSrgbBlockImg = 1000054007,
        G8b8g8r8422UnormKHR = 1000156000,
        B8g8r8g8422UnormKHR = 1000156001,
        G8B8R83plane420UnormKHR = 1000156002,
        G8B8r82plane420UnormKHR = 1000156003,
        G8B8R83plane422UnormKHR = 1000156004,
        G8B8r82plane422UnormKHR = 1000156005,
        G8B8R83plane444UnormKHR = 1000156006,
        R10x6UnormPack16KHR = 1000156007,
        R10x6g10x6Unorm2pack16KHR = 1000156008,
        R10x6g10x6b10x6a10x6Unorm4pack16KHR = 1000156009,
        G10x6b10x6g10x6r10x6422Unorm4pack16KHR = 1000156010,
        B10x6g10x6r10x6g10x6422Unorm4pack16KHR = 1000156011,
        G10x6B10x6R10x63plane420Unorm3pack16KHR = 1000156012,
        G10x6B10x6r10x62plane420Unorm3pack16KHR = 1000156013,
        G10x6B10x6R10x63plane422Unorm3pack16KHR = 1000156014,
        G10x6B10x6r10x62plane422Unorm3pack16KHR = 1000156015,
        G10x6B10x6R10x63plane444Unorm3pack16KHR = 1000156016,
        R12x4UnormPack16KHR = 1000156017,
        R12x4g12x4Unorm2pack16KHR = 1000156018,
        R12x4g12x4b12x4a12x4Unorm4pack16KHR = 1000156019,
        G12x4b12x4g12x4r12x4422Unorm4pack16KHR = 1000156020,
        B12x4g12x4r12x4g12x4422Unorm4pack16KHR = 1000156021,
        G12x4B12x4R12x43plane420Unorm3pack16KHR = 1000156022,
        G12x4B12x4r12x42plane420Unorm3pack16KHR = 1000156023,
        G12x4B12x4R12x43plane422Unorm3pack16KHR = 1000156024,
        G12x4B12x4r12x42plane422Unorm3pack16KHR = 1000156025,
        G12x4B12x4R12x43plane444Unorm3pack16KHR = 1000156026,
        G16b16g16r16422UnormKHR = 1000156027,
        B16g16r16g16422UnormKHR = 1000156028,
        G16B16R163plane420UnormKHR = 1000156029,
        G16B16r162plane420UnormKHR = 1000156030,
        G16B16R163plane422UnormKHR = 1000156031,
        G16B16r162plane422UnormKHR = 1000156032,
        G16B16R163plane444UnormKHR = 1000156033
    }

}
