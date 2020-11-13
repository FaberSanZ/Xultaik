// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	VulkanConvert.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Vulkan;

namespace Zeckoxe.Graphics
{
    public static class VulkanConvert
    {

        internal static VkPrimitiveTopology ToPrimitiveType(this PrimitiveType primitiveType)
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

        internal static MultisampleCount ToSampleCountFlags_Invert(this VkSampleCountFlags countFlags)
        {
            switch (countFlags)
            {
                case VkSampleCountFlags.None:
                    return MultisampleCount.None;

                case VkSampleCountFlags.Count1:
                    return MultisampleCount.X1;

                case VkSampleCountFlags.Count2:
                    return MultisampleCount.X2;

                case VkSampleCountFlags.Count4:
                    return MultisampleCount.X4;

                case VkSampleCountFlags.Count8:
                    return MultisampleCount.X8;

                case VkSampleCountFlags.Count16:
                    return MultisampleCount.X16;

                case VkSampleCountFlags.Count32:
                    return MultisampleCount.X32;

                case VkSampleCountFlags.Count64:
                    return MultisampleCount.X64;

                default:
                    throw new ArgumentOutOfRangeException(nameof(countFlags));
            }
        }

        internal static VkPolygonMode ToFillMode(this FillMode fillMode)
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

        internal static VkFrontFace ToFrontFace(this FrontFace frontFace)
        {
            switch (frontFace)
            {
                case FrontFace.Clockwise:
                    return VkFrontFace.Clockwise;

                case FrontFace.CounterClockwise:
                    return VkFrontFace.CounterClockwise;


                default:
                    throw new ArgumentOutOfRangeException(nameof(frontFace));
            }
        }


        internal static VkCullModeFlags ToCullMode(this CullMode cullMode)
        {
            switch (cullMode)
            {
                case CullMode.Back:
                    return VkCullModeFlags.Back;

                case CullMode.Front:
                    return VkCullModeFlags.Front;

                case CullMode.FrontAndBack:
                    return VkCullModeFlags.FrontAndBack;

                case CullMode.None:
                    return VkCullModeFlags.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cullMode));
            }
        }

        internal static VkImageAspectFlags format_to_aspect_mask(VkFormat format)
        {
            throw new NotImplementedException();
        }
    }
}
