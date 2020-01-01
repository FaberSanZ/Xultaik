// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	VulkanConvert.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vulkan;

namespace Zeckoxe.Graphics
{
    public static class VulkanConvert
    {
        internal static VkPhysicalDeviceType ToDeviceType(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Other:
                    return VkPhysicalDeviceType.Other;

                case DeviceType.IntegratedGPU:
                    return VkPhysicalDeviceType.Other;

                case DeviceType.DiscreteGPU:
                    return VkPhysicalDeviceType.Other;

                case DeviceType.VirtualGPU:
                    return VkPhysicalDeviceType.Other;

                case DeviceType.CPU:
                    return VkPhysicalDeviceType.Other;

                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceType));
            }
        }

        internal static VkPrimitiveTopology ToPrimitiveType(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return (VkPrimitiveTopology.PointList);

                case PrimitiveType.LineList:
                    return VkPrimitiveTopology.LineList;

                case PrimitiveType.LineStrip:
                    return VkPrimitiveTopology.LineStrip;

                case PrimitiveType.TriangleList:
                    return VkPrimitiveTopology.TriangleList;

                case PrimitiveType.TriangleStrip:
                    return VkPrimitiveTopology.TriangleStrip;

                case PrimitiveType.LineListWithAdjacency:
                    return VkPrimitiveTopology.LineListWithAdjacency;

                case PrimitiveType.LineStripWithAdjacency:
                    return VkPrimitiveTopology.LineStripWithAdjacency;

                case PrimitiveType.TriangleListWithAdjacency:
                    return VkPrimitiveTopology.TriangleListWithAdjacency;

                case PrimitiveType.TriangleStripWithAdjacency:
                    return VkPrimitiveTopology.TriangleStripWithAdjacency;

                default:
                    throw new ArgumentOutOfRangeException(nameof(primitiveType));
            }
        }



        internal static VkPolygonMode ToFillMode(FillMode fillMode)
        {
            switch (fillMode)
            {
                case FillMode.Solid:
                    return VkPolygonMode.Fill;

                case FillMode.Wireframe:
                    return VkPolygonMode.Line;

                case FillMode.Point:
                    return VkPolygonMode.Point;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fillMode));
            }
        }


        internal static VkCullModeFlags ToCullMode(CullMode cullMode)
        {
            switch (cullMode)
            {
                case CullMode.Back:
                    return VkCullModeFlags.Back;

                case CullMode.Front:
                    return VkCullModeFlags.Front;

                case CullMode.None:
                    return VkCullModeFlags.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cullMode));
            }
        }


        internal static int BlockSizeInBytes(PixelFormat format)
        {
            return 1 << 8 - 1 | 1 >> 8 + 1 * (int)format * (nameof(PixelFormat).Length); // LOL
        }


        internal static VkFormat ToPixelFormat(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Undefined:
                    return VkFormat.Undefined;

                case PixelFormat.R4G4_UNorm_Pack8:
                    return VkFormat.R4g4UnormPack8;

                case PixelFormat.R4G4B4A4_UNorm_Pack16:
                    return VkFormat.R4g4b4a4UnormPack16;

                case PixelFormat.B4G4R4A4_UNorm_Pack16:
                    return VkFormat.B4g4r4a4UnormPack16;

                case PixelFormat.R5G6B5_UNorm_Pack16:
                    return VkFormat.R5g6b5UnormPack16;

                case PixelFormat.B5G6R5_UNorm_Pack16:
                    return VkFormat.B5g6r5UnormPack16;

                case PixelFormat.R5G5b5A1_UNorm_Pack16:
                    return VkFormat.R5g5b5a1UnormPack16;

                case PixelFormat.B5G5R5A1_UNorm_Pack16:
                    return VkFormat.B5g5r5a1UnormPack16;

                case PixelFormat.A1R5G5B5_UNorm_Pack16:
                    return VkFormat.A1r5g5b5UnormPack16;

                case PixelFormat.R8_UNorm:
                    return VkFormat.R8Unorm;

                case PixelFormat.R8_SNorm:
                    return VkFormat.R8Snorm;

                case PixelFormat.R8_UScaled:
                    return VkFormat.R8Uscaled;

                case PixelFormat.R8_SScaled:
                    return VkFormat.R8Sscaled;

                case PixelFormat.R8_Uint:
                    return VkFormat.R8Uint;

                case PixelFormat.R8_SInt:
                    return VkFormat.R8Sint;

                case PixelFormat.R8_SRGB:
                    return VkFormat.R8Srgb;

                case PixelFormat.R8G8_UNorm:
                    return VkFormat.R8g8Unorm;

                case PixelFormat.R8G8_SNorm:
                    return VkFormat.R8g8Snorm;

                case PixelFormat.R8G8_UScaled:
                    return VkFormat.R8g8Uscaled;

                case PixelFormat.R8G8_SScaled:
                    return VkFormat.R8g8Sscaled;

                case PixelFormat.R8G8_UInt:
                    return VkFormat.R8g8Uint;

                case PixelFormat.R8G8_SInt:
                    return VkFormat.R8g8Sint;

                case PixelFormat.R8G8_SRGB:
                    return VkFormat.R8g8Srgb;

                case PixelFormat.R8G8B8_UNorm:
                    return VkFormat.R8g8b8Unorm;

                case PixelFormat.R8G8B8_SNorm:
                    return VkFormat.R8g8b8Snorm;

                case PixelFormat.R8g8b8Uscaled:
                    return VkFormat.R8g8b8Uscaled;

                case PixelFormat.R8g8b8Sscaled:
                    return VkFormat.R8g8b8Sscaled;

                case PixelFormat.R8g8b8Uint:
                    return VkFormat.R8g8b8Uint;

                case PixelFormat.R8g8b8Sint:
                    return VkFormat.R8g8b8Sint;

                case PixelFormat.R8g8b8Srgb:
                    return VkFormat.R8g8b8Srgb;

                case PixelFormat.B8g8r8Unorm:
                    return VkFormat.B8g8r8Unorm;

                case PixelFormat.B8g8r8Snorm:
                    return VkFormat.B8g8r8Snorm;

                case PixelFormat.B8g8r8Uscaled:
                    return VkFormat.B8g8r8Uscaled;

                case PixelFormat.B8g8r8Sscaled:
                    return VkFormat.B8g8r8Sscaled;

                case PixelFormat.B8g8r8Uint:
                    return VkFormat.B8g8r8Uint;

                case PixelFormat.B8g8r8Sint:
                    return VkFormat.B8g8r8Sint;

                case PixelFormat.B8g8r8Srgb:
                    return VkFormat.B8g8r8Srgb;

                case PixelFormat.R8g8b8a8Unorm:
                    return VkFormat.R8g8b8a8Unorm;

                case PixelFormat.R8g8b8a8Snorm:
                    return VkFormat.R8g8b8a8Snorm;

                case PixelFormat.R8g8b8a8Uscaled:
                    return VkFormat.R8g8b8a8Uscaled;

                case PixelFormat.R8g8b8a8Sscaled:
                    return VkFormat.R8g8b8a8Sscaled;

                case PixelFormat.R8g8b8a8Uint:
                    return VkFormat.R8g8b8a8Uint;

                case PixelFormat.R8g8b8a8Sint:
                    return VkFormat.R8g8b8a8Sint;

                case PixelFormat.R8g8b8a8Srgb:
                    return VkFormat.R8g8b8a8Srgb;

                case PixelFormat.B8g8r8a8Unorm:
                    return VkFormat.B8g8r8a8Unorm;

                case PixelFormat.B8g8r8a8Snorm:
                    return VkFormat.B8g8r8a8Snorm;

                case PixelFormat.B8g8r8a8Uscaled:
                    return VkFormat.B8g8r8a8Uscaled;

                case PixelFormat.B8g8r8a8Sscaled:
                    return VkFormat.B8g8r8a8Sscaled;

                case PixelFormat.B8g8r8a8Uint:
                    return VkFormat.B8g8r8a8Uint;

                case PixelFormat.B8g8r8a8Sint:
                    return VkFormat.B8g8r8a8Sint;

                case PixelFormat.B8g8r8a8Srgb:
                    return VkFormat.B8g8r8a8Srgb;

                case PixelFormat.A8b8g8r8UnormPack32:
                    return VkFormat.A8b8g8r8UnormPack32;

                case PixelFormat.A8b8g8r8SnormPack32:
                    return VkFormat.A8b8g8r8SnormPack32;

                case PixelFormat.A8b8g8r8UscaledPack32:
                    return VkFormat.A8b8g8r8UscaledPack32;

                case PixelFormat.A8b8g8r8SscaledPack32:
                    return VkFormat.A8b8g8r8SscaledPack32;

                case PixelFormat.A8b8g8r8UintPack32:
                    return VkFormat.A8b8g8r8UintPack32;

                case PixelFormat.A8b8g8r8SintPack32:
                    return VkFormat.A8b8g8r8SintPack32;

                case PixelFormat.A8b8g8r8SrgbPack32:
                    return VkFormat.A8b8g8r8SrgbPack32;

                case PixelFormat.A2r10g10b10UnormPack32:
                    return VkFormat.A2r10g10b10UnormPack32;

                case PixelFormat.A2r10g10b10SnormPack32:
                    return VkFormat.A2r10g10b10SnormPack32;

                case PixelFormat.A2r10g10b10UscaledPack32:
                    return VkFormat.A2r10g10b10UscaledPack32;

                case PixelFormat.A2r10g10b10SscaledPack32:
                    return VkFormat.A2r10g10b10SscaledPack32;

                case PixelFormat.A2r10g10b10UintPack32:
                    return VkFormat.A2r10g10b10UintPack32;

                case PixelFormat.A2r10g10b10SintPack32:
                    return VkFormat.A2r10g10b10SintPack32;

                case PixelFormat.A2b10g10r10UnormPack32:
                    return VkFormat.A2b10g10r10UnormPack32;

                case PixelFormat.A2b10g10r10SnormPack32:
                    return VkFormat.A2b10g10r10SnormPack32;

                case PixelFormat.A2b10g10r10UscaledPack32:
                    return VkFormat.A2b10g10r10UscaledPack32;

                case PixelFormat.A2b10g10r10SscaledPack32:
                    return VkFormat.A2b10g10r10SscaledPack32;

                case PixelFormat.A2b10g10r10UintPack32:
                    return VkFormat.A2b10g10r10UintPack32;

                case PixelFormat.A2b10g10r10SintPack32:
                    return VkFormat.A2b10g10r10SintPack32;

                case PixelFormat.R16Unorm:
                    return VkFormat.R16Unorm;

                case PixelFormat.R16Snorm:
                    return VkFormat.R16Snorm;

                case PixelFormat.R16Uscaled:
                    return VkFormat.R16Uscaled;

                case PixelFormat.R16Sscaled:
                    return VkFormat.R16Sscaled;

                case PixelFormat.R16Uint:
                    return VkFormat.R16Uint;

                case PixelFormat.R16Sint:
                    return VkFormat.R16Sint;

                case PixelFormat.R16Sfloat:
                    return VkFormat.R16Sfloat;

                case PixelFormat.R16g16Unorm:
                    return VkFormat.R16g16Unorm;

                case PixelFormat.R16g16Snorm:
                    return VkFormat.R16g16Snorm;

                case PixelFormat.R16g16Uscaled:
                    return VkFormat.R16g16Uscaled;

                case PixelFormat.R16g16Sscaled:
                    return VkFormat.R16g16Sscaled;

                case PixelFormat.R16g16Uint:
                    return VkFormat.R16g16Uint;

                case PixelFormat.R16g16Sint:
                    return VkFormat.R16g16Sint;

                case PixelFormat.R16g16Sfloat:
                    return VkFormat.R16g16Sfloat;

                case PixelFormat.R16g16b16Unorm:
                    return VkFormat.R16g16b16Unorm;

                case PixelFormat.R16g16b16Snorm:
                    return VkFormat.R16g16b16Snorm;

                case PixelFormat.R16g16b16Uscaled:
                    return VkFormat.R16g16b16Uscaled;

                case PixelFormat.R16g16b16Sscaled:
                    return VkFormat.R16g16b16Sscaled;

                case PixelFormat.R16g16b16Uint:
                    return VkFormat.R16g16b16Uint;

                case PixelFormat.R16g16b16Sint:
                    return VkFormat.R16g16b16Sint;

                case PixelFormat.R16g16b16Sfloat:
                    return VkFormat.R16g16b16Sfloat;

                case PixelFormat.R16g16b16a16Unorm:
                    return VkFormat.R16g16b16a16Unorm;

                case PixelFormat.R16g16b16a16Snorm:
                    return VkFormat.R16g16b16a16Snorm;

                case PixelFormat.R16g16b16a16Uscaled:
                    return VkFormat.R16g16b16a16Uscaled;

                case PixelFormat.R16g16b16a16Sscaled:
                    return VkFormat.R16g16b16a16Sscaled;

                case PixelFormat.R16g16b16a16Uint:
                    return VkFormat.R16g16b16a16Uint;

                case PixelFormat.R16g16b16a16Sint:
                    return VkFormat.R16g16b16a16Sint;

                case PixelFormat.R16g16b16a16Sfloat:
                    return VkFormat.R16g16b16a16Sfloat;

                case PixelFormat.R32Uint:
                    return VkFormat.R32Uint;

                case PixelFormat.R32Sint:
                    return VkFormat.R32Sint;

                case PixelFormat.R32Sfloat:
                    return VkFormat.R32Sfloat;

                case PixelFormat.R32g32Uint:
                    return VkFormat.R32g32Uint;

                case PixelFormat.R32g32Sint:
                    return VkFormat.R32g32Sint;

                case PixelFormat.R32g32Sfloat:
                    return VkFormat.R32g32Sfloat;

                case PixelFormat.R32g32b32Uint:
                    return VkFormat.R32g32b32Uint;

                case PixelFormat.R32g32b32Sint:
                    return VkFormat.R32g32b32Sint;

                case PixelFormat.R32g32b32Sfloat:
                    return VkFormat.R32g32b32Sfloat;

                case PixelFormat.R32g32b32a32Uint:
                    return VkFormat.R32g32b32a32Uint;

                case PixelFormat.R32g32b32a32Sint:
                    return VkFormat.R32g32b32a32Sint;

                case PixelFormat.R32g32b32a32Sfloat:
                    return VkFormat.R32g32b32a32Sfloat;

                case PixelFormat.R64Uint:
                    return VkFormat.R64Uint;

                case PixelFormat.R64Sint:
                    return VkFormat.R64Sint;

                case PixelFormat.R64Sfloat:
                    return VkFormat.R64Sfloat;

                case PixelFormat.R64g64Uint:
                    return VkFormat.R64g64Uint;

                case PixelFormat.R64g64Sint:
                    return VkFormat.R64g64Sint;

                case PixelFormat.R64g64Sfloat:
                    return VkFormat.R64g64Sfloat;

                case PixelFormat.R64g64b64Uint:
                    return VkFormat.R64g64b64Uint;

                case PixelFormat.R64g64b64Sint:
                    return VkFormat.R64g64b64Sint;

                case PixelFormat.R64g64b64Sfloat:
                    return VkFormat.R64g64b64Sfloat;

                case PixelFormat.R64g64b64a64Uint:
                    return VkFormat.R64g64b64a64Uint;

                case PixelFormat.R64g64b64a64Sint:
                    return VkFormat.R64g64b64a64Sint;

                case PixelFormat.R64g64b64a64Sfloat:
                    return VkFormat.R64g64b64a64Sfloat;

                case PixelFormat.B10g11r11UfloatPack32:
                    return VkFormat.B10g11r11UfloatPack32;

                case PixelFormat.E5b9g9r9UfloatPack32:
                    return VkFormat.E5b9g9r9UfloatPack32;

                case PixelFormat.D16Unorm:
                    return VkFormat.D16Unorm;

                case PixelFormat.X8D24UnormPack32:
                    return VkFormat.X8D24UnormPack32;

                case PixelFormat.D32Sfloat:
                    return VkFormat.D32Sfloat;

                case PixelFormat.S8Uint:
                    return VkFormat.S8Uint;

                case PixelFormat.D16UnormS8Uint:
                    return VkFormat.D16UnormS8Uint;

                case PixelFormat.D24UnormS8Uint:
                    return VkFormat.D24UnormS8Uint;

                case PixelFormat.D32SfloatS8Uint:
                    return VkFormat.D32SfloatS8Uint;

                case PixelFormat.Bc1RgbUnormBlock:
                    return VkFormat.Bc1RgbUnormBlock;

                case PixelFormat.Bc1RgbSrgbBlock:
                    return VkFormat.Bc1RgbSrgbBlock;

                case PixelFormat.Bc1RgbaUnormBlock:
                    return VkFormat.Bc1RgbaUnormBlock;

                case PixelFormat.Bc1RgbaSrgbBlock:
                    return VkFormat.Bc1RgbaSrgbBlock;

                case PixelFormat.Bc2UnormBlock:
                    return VkFormat.Bc2UnormBlock;

                case PixelFormat.Bc2SrgbBlock:
                    return VkFormat.Bc2SrgbBlock;

                case PixelFormat.Bc3UnormBlock:
                    return VkFormat.Bc3UnormBlock;

                case PixelFormat.Bc3SrgbBlock:
                    return VkFormat.Bc3SrgbBlock;

                case PixelFormat.Bc4UnormBlock:
                    return VkFormat.Bc4UnormBlock;

                case PixelFormat.Bc4SnormBlock:
                    return VkFormat.Bc4SnormBlock;

                case PixelFormat.Bc5UnormBlock:
                    return VkFormat.Bc5UnormBlock;

                case PixelFormat.Bc5SnormBlock:
                    return VkFormat.Bc5SnormBlock;

                case PixelFormat.Bc6hUfloatBlock:
                    return VkFormat.Bc6hUfloatBlock;

                case PixelFormat.Bc6hSfloatBlock:
                    return VkFormat.Bc6hSfloatBlock;

                case PixelFormat.Bc7UnormBlock:
                    return VkFormat.Bc7UnormBlock;

                case PixelFormat.Bc7SrgbBlock:
                    return VkFormat.Bc7SrgbBlock;

                case PixelFormat.Etc2R8g8b8UnormBlock:
                    return VkFormat.Etc2R8g8b8UnormBlock;

                case PixelFormat.Etc2R8g8b8SrgbBlock:
                    return VkFormat.Etc2R8g8b8SrgbBlock;

                case PixelFormat.Etc2R8g8b8a1UnormBlock:
                    return VkFormat.Etc2R8g8b8a1UnormBlock;

                case PixelFormat.Etc2R8g8b8a1SrgbBlock:
                    return VkFormat.Etc2R8g8b8a1SrgbBlock;

                case PixelFormat.Etc2R8g8b8a8UnormBlock:
                    return VkFormat.Etc2R8g8b8a8UnormBlock;

                case PixelFormat.Etc2R8g8b8a8SrgbBlock:
                    return VkFormat.Etc2R8g8b8a8SrgbBlock;

                case PixelFormat.EacR11UnormBlock:
                    return VkFormat.EacR11UnormBlock;

                case PixelFormat.EacR11SnormBlock:
                    return VkFormat.EacR11SnormBlock;

                case PixelFormat.EacR11g11UnormBlock:
                    return VkFormat.EacR11g11UnormBlock;

                case PixelFormat.EacR11g11SnormBlock:
                    return VkFormat.EacR11g11SnormBlock;

                case PixelFormat.Astc4x4UnormBlock:
                    return VkFormat.Astc4x4UnormBlock;

                case PixelFormat.Astc4x4SrgbBlock:
                    return VkFormat.Astc4x4SrgbBlock;

                case PixelFormat.Astc5x4UnormBlock:
                    return VkFormat.Undefined;

                case PixelFormat.Astc5x4SrgbBlock:
                    return VkFormat.Astc5x4SrgbBlock;

                case PixelFormat.Astc5x5UnormBlock:
                    return VkFormat.Astc5x5UnormBlock;

                case PixelFormat.Astc5x5SrgbBlock:
                    return VkFormat.Astc5x5SrgbBlock;

                case PixelFormat.Astc6x5UnormBlock:
                    return VkFormat.Astc6x5UnormBlock;

                case PixelFormat.Astc6x5SrgbBlock:
                    return VkFormat.Astc6x5SrgbBlock;

                case PixelFormat.Astc6x6UnormBlock:
                    return VkFormat.Astc6x6UnormBlock;

                case PixelFormat.Astc6x6SrgbBlock:
                    return VkFormat.Astc6x6SrgbBlock;

                case PixelFormat.Astc8x5UnormBlock:
                    return VkFormat.Astc8x5UnormBlock;

                case PixelFormat.Astc8x5SrgbBlock:
                    return VkFormat.Astc8x5SrgbBlock;

                case PixelFormat.Astc8x6UnormBlock:
                    return VkFormat.Astc8x6UnormBlock;

                case PixelFormat.Astc8x6SrgbBlock:
                    return VkFormat.Astc8x6SrgbBlock;

                case PixelFormat.Astc8x8UnormBlock:
                    return VkFormat.Astc8x8UnormBlock;

                case PixelFormat.Astc8x8SrgbBlock:
                    return VkFormat.Astc8x8SrgbBlock;

                case PixelFormat.Astc10x5UnormBlock:
                    return VkFormat.Astc10x5UnormBlock;

                case PixelFormat.Astc10x5SrgbBlock:
                    return VkFormat.Astc10x5SrgbBlock;

                case PixelFormat.Astc10x6UnormBlock:
                    return VkFormat.Astc10x6UnormBlock;

                case PixelFormat.Astc10x6SrgbBlock:
                    return VkFormat.Astc10x6SrgbBlock;

                case PixelFormat.Astc10x8UnormBlock:
                    return VkFormat.Astc10x8UnormBlock;

                case PixelFormat.Astc10x8SrgbBlock:
                    return VkFormat.Astc10x8SrgbBlock;

                case PixelFormat.Astc10x10UnormBlock:
                    return VkFormat.Astc10x10UnormBlock;

                case PixelFormat.Astc10x10SrgbBlock:
                    return VkFormat.Astc10x10SrgbBlock;

                case PixelFormat.Astc12x10UnormBlock:
                    return VkFormat.Astc12x10UnormBlock;

                case PixelFormat.Astc12x10SrgbBlock:
                    return VkFormat.Astc12x10SrgbBlock;

                case PixelFormat.Astc12x12UnormBlock:
                    return VkFormat.Astc12x12UnormBlock;

                case PixelFormat.Astc12x12SrgbBlock:
                    return VkFormat.Astc12x12SrgbBlock;

                case PixelFormat.Pvrtc12bppUnormBlockImg:
                    return VkFormat.Pvrtc12bppUnormBlockImg;

                case PixelFormat.Pvrtc14bppUnormBlockImg:
                    return VkFormat.Pvrtc14bppUnormBlockImg;

                case PixelFormat.Pvrtc22bppUnormBlockImg:
                    return VkFormat.Pvrtc22bppUnormBlockImg;

                case PixelFormat.Pvrtc24bppUnormBlockImg:
                    return VkFormat.Pvrtc24bppUnormBlockImg;

                case PixelFormat.Pvrtc12bppSrgbBlockImg:
                    return VkFormat.Pvrtc12bppSrgbBlockImg;

                case PixelFormat.Pvrtc14bppSrgbBlockImg:
                    return VkFormat.Pvrtc14bppSrgbBlockImg;

                case PixelFormat.Pvrtc22bppSrgbBlockImg:
                    return VkFormat.Pvrtc22bppSrgbBlockImg;

                case PixelFormat.Pvrtc24bppSrgbBlockImg:
                    return VkFormat.Pvrtc24bppSrgbBlockImg;

                case PixelFormat.G8b8g8r8422UnormKHR:
                    return VkFormat.G8b8g8r8422UnormKHR;

                case PixelFormat.B8g8r8g8422UnormKHR:
                    return VkFormat.B8g8r8g8422UnormKHR;

                case PixelFormat.G8B8R83plane420UnormKHR:
                    return VkFormat.G8B8R83plane420UnormKHR;

                case PixelFormat.G8B8r82plane420UnormKHR:
                    return VkFormat.G8B8r82plane420UnormKHR;

                case PixelFormat.G8B8R83plane422UnormKHR:
                    return VkFormat.G8B8R83plane422UnormKHR;

                case PixelFormat.G8B8r82plane422UnormKHR:
                    return VkFormat.G8B8r82plane422UnormKHR;

                case PixelFormat.G8B8R83plane444UnormKHR:
                    return VkFormat.G8B8R83plane444UnormKHR;

                case PixelFormat.R10x6UnormPack16KHR:
                    return VkFormat.R10x6UnormPack16KHR;

                case PixelFormat.R10x6g10x6Unorm2pack16KHR:
                    return VkFormat.R10x6g10x6Unorm2pack16KHR;

                case PixelFormat.R10x6g10x6b10x6a10x6Unorm4pack16KHR:
                    return VkFormat.R10x6g10x6b10x6a10x6Unorm4pack16KHR;

                case PixelFormat.G10x6b10x6g10x6r10x6422Unorm4pack16KHR:
                    return VkFormat.G10x6b10x6g10x6r10x6422Unorm4pack16KHR;

                case PixelFormat.B10x6g10x6r10x6g10x6422Unorm4pack16KHR:
                    return VkFormat.B10x6g10x6r10x6g10x6422Unorm4pack16KHR;

                case PixelFormat.G10x6B10x6R10x63plane420Unorm3pack16KHR:
                    return VkFormat.G10x6B10x6R10x63plane420Unorm3pack16KHR;

                case PixelFormat.G10x6B10x6r10x62plane420Unorm3pack16KHR:
                    return VkFormat.G10x6B10x6r10x62plane420Unorm3pack16KHR;

                case PixelFormat.G10x6B10x6R10x63plane422Unorm3pack16KHR:
                    return VkFormat.G10x6B10x6R10x63plane422Unorm3pack16KHR;

                case PixelFormat.G10x6B10x6r10x62plane422Unorm3pack16KHR:
                    return VkFormat.G10x6B10x6r10x62plane422Unorm3pack16KHR;

                case PixelFormat.G10x6B10x6R10x63plane444Unorm3pack16KHR:
                    return VkFormat.G10x6B10x6R10x63plane444Unorm3pack16KHR;

                case PixelFormat.R12x4UnormPack16KHR:
                    return VkFormat.R12x4UnormPack16KHR;

                case PixelFormat.R12x4g12x4Unorm2pack16KHR:
                    return VkFormat.R12x4g12x4Unorm2pack16KHR;

                case PixelFormat.R12x4g12x4b12x4a12x4Unorm4pack16KHR:
                    return VkFormat.R12x4g12x4b12x4a12x4Unorm4pack16KHR;

                case PixelFormat.G12x4b12x4g12x4r12x4422Unorm4pack16KHR:
                    return VkFormat.G12x4b12x4g12x4r12x4422Unorm4pack16KHR;

                case PixelFormat.B12x4g12x4r12x4g12x4422Unorm4pack16KHR:
                    return VkFormat.B12x4g12x4r12x4g12x4422Unorm4pack16KHR;

                case PixelFormat.G12x4B12x4R12x43plane420Unorm3pack16KHR:
                    return VkFormat.G12x4B12x4R12x43plane420Unorm3pack16KHR;

                case PixelFormat.G12x4B12x4r12x42plane420Unorm3pack16KHR:
                    return VkFormat.G12x4B12x4r12x42plane420Unorm3pack16KHR;

                case PixelFormat.G12x4B12x4R12x43plane422Unorm3pack16KHR:
                    return VkFormat.G12x4B12x4R12x43plane422Unorm3pack16KHR;

                case PixelFormat.G12x4B12x4r12x42plane422Unorm3pack16KHR:
                    return VkFormat.G12x4B12x4r12x42plane422Unorm3pack16KHR;

                case PixelFormat.G12x4B12x4R12x43plane444Unorm3pack16KHR:
                    return VkFormat.G12x4B12x4R12x43plane444Unorm3pack16KHR;

                case PixelFormat.G16b16g16r16422UnormKHR:
                    return VkFormat.G16b16g16r16422UnormKHR;

                case PixelFormat.B16g16r16g16422UnormKHR:
                    return VkFormat.B16g16r16g16422UnormKHR;

                case PixelFormat.G16B16R163plane420UnormKHR:
                    return VkFormat.G16B16R163plane420UnormKHR;

                case PixelFormat.G16B16r162plane420UnormKHR:
                    return VkFormat.G16B16r162plane420UnormKHR;

                case PixelFormat.G16B16R163plane422UnormKHR:
                    return VkFormat.G16B16R163plane422UnormKHR;

                case PixelFormat.G16B16r162plane422UnormKHR:
                    return VkFormat.G16B16r162plane422UnormKHR;

                case PixelFormat.G16B16R163plane444UnormKHR:
                    return VkFormat.G16B16R163plane444UnormKHR;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format));
            }


        }
    }
}
