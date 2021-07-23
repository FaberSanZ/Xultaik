// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)






using Vortice.ShaderCompiler;
using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;
using SPIRVCross;
using System.Numerics;
using System.Diagnostics;

namespace Vultaik
{
    public static unsafe class Helpers
    {
        public static string ExtractVersion(uint _value)
        {

            uint major = _value >> 22;
            uint minor = (_value >> 12) & 0x03FF;
            uint patch = _value & 0x0FFF;

            return $"{major}.{minor}.{patch}";
        }


        // https://github.com/HansKristian-Work/vkd3d-proton/blob/master/libs/vkd3d/vkd3d_private.h#L2954
        public static int ComputeWorkGroupCount(int threadCount, int workGroupSize)
        {
            return (threadCount + workGroupSize - 1) / workGroupSize;
        }



        public static int ConvertToSignedDelta(int start_ticks, int end_ticks, int valid_bits)
        {
            int shamt = 64 - valid_bits;
            start_ticks <<= shamt;
            end_ticks <<= shamt;
            int ticks_delta = end_ticks - start_ticks;
            ticks_delta >>= shamt;
            return ticks_delta;
        }

        public static bool IsPow2(int v)
        {
            return BitOperations.PopCount((uint)v) == 1;
        }

        public static bool IsPow2(long v)
        {
            return BitOperations.PopCount((ulong)v) == 1;
        }

        public static int NextPow2(int v)
        {
            if (IsPow2(v))
                return v;

            return 1 << (32 - BitOperations.LeadingZeroCount((uint)v));
        }

        public static long NextPow2(long v)
        {
            if (IsPow2(v))
                return v;

            return 1L << (64 - BitOperations.LeadingZeroCount((ulong)v));
        }

        public static int PrevPow(int v)
        {
            return 1 << (31 - BitOperations.LeadingZeroCount((uint)v));
        }

        public static long PrevPow(long v)
        {
            return 1L << (63 - BitOperations.LeadingZeroCount((ulong)v));
        }

        public static bool BlocksOnSamePage(long resourceAOffset, long resourceASize, long resourceBOffset, long pageSize)
        {
            Debug.Assert(resourceAOffset + resourceASize <= resourceBOffset && resourceASize > 0 && pageSize > 0);

            long resourceAEnd = resourceAOffset + resourceASize - 1;
            long resourceAEndPage = resourceAEnd & ~(pageSize - 1);
            long resourceBStart = resourceBOffset;
            long resourceBStartPage = resourceBStart & ~(pageSize - 1);

            return resourceAEndPage == resourceBStartPage;
        }

        public static long AlignUp(long value, long alignment)
        {
            return (value + alignment - 1) / alignment * alignment;
        }

        public static long AlignDown(long value, long alignment)
        {
            return (long)((ulong)value / (ulong)alignment * (ulong)alignment);
        }



        internal static VkCullModeFlags ConvertCullMode(this CullMode cullMode)
        {
            // TODO: Vulkan's VkCullModeFlags.FrontAndBack is not exposed

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



        internal static VkVertexInputRate ConvertVertexInputRate(this VertexInputRate rate)
        {

            switch (rate)
            {
                case VertexInputRate.Vertex:
                    return VkVertexInputRate.Vertex;

                case VertexInputRate.Instance:
                    return VkVertexInputRate.Instance;

                default:
                    throw new ArgumentOutOfRangeException(nameof(rate));
            }
        }


        internal static VkFrontFace ConvertFrontFace(this FrontFace frontFace)
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

        

        public static bool PrimitiveRestart(this PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                case PrimitiveType.LineList:
                case PrimitiveType.TriangleList:
                case PrimitiveType.LineListWithAdjacency:
                case PrimitiveType.TriangleListWithAdjacency:
                case PrimitiveType.PatchList:
                    return false;

                default:
                    return true;
            }
        }

        internal static VkPrimitiveTopology ConvertPrimitiveType(this PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return VkPrimitiveTopology.PointList;
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


        internal static VkPolygonMode FillModeToVkPolygonMode(this FillMode fillMode)
        {
            // TODO: Vulkan's PolygonMode.Point is not exposed

            switch (fillMode)
            {
                case FillMode.Solid:
                    return VkPolygonMode.Fill;

                case FillMode.Wireframe:
                    return VkPolygonMode.Line;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fillMode));
            }
        }


        internal static VkDescriptorType StageTVkDescriptorType(this spvc_resource_type stage, bool is_dynamic)
        {
            switch (stage)
            {
                case spvc_resource_type.UniformBuffer:
                    return is_dynamic ? VkDescriptorType.UniformBufferDynamic : VkDescriptorType.UniformBuffer;

                case spvc_resource_type.StorageBuffer:
                    return VkDescriptorType.StorageBuffer;


                case spvc_resource_type.StorageImage:
                    return VkDescriptorType.StorageImage;

                case spvc_resource_type.SampledImage:
                    return VkDescriptorType.CombinedImageSampler;

                case spvc_resource_type.SeparateImage:
                    return VkDescriptorType.SampledImage;

                case spvc_resource_type.SeparateSamplers:
                    return VkDescriptorType.Sampler;

                case spvc_resource_type.SubpassInput:
                    return VkDescriptorType.InputAttachment;

                case spvc_resource_type.AccelerationStructure:
                    return VkDescriptorType.AccelerationStructureKHR;


                default:
                    return 0;

            }
        }

        internal static VkShaderStageFlags StageToVkShaderStageFlags(this ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return VkShaderStageFlags.Vertex;

                case ShaderStage.Fragment:
                    return VkShaderStageFlags.Fragment;

                case ShaderStage.Compute:
                    return VkShaderStageFlags.Compute;

                case ShaderStage.Geometry:
                    return VkShaderStageFlags.Geometry;

                case ShaderStage.TessellationControl:
                    return VkShaderStageFlags.TessellationControl;

                case ShaderStage.TessellationEvaluation:
                    return VkShaderStageFlags.TessellationEvaluation;

                case ShaderStage.Raygen:
                    return VkShaderStageFlags.RaygenKHR;

                case ShaderStage.AnyHit:
                    return VkShaderStageFlags.AnyHitKHR;

                case ShaderStage.ClosestHit:
                    return VkShaderStageFlags.ClosestHitKHR;

                case ShaderStage.Miss:
                    return VkShaderStageFlags.MissKHR;

                case ShaderStage.Intersection:
                    return VkShaderStageFlags.IntersectionKHR;

                case ShaderStage.Callable:
                    return VkShaderStageFlags.CallableKHR;

                case ShaderStage.TaskNV:
                    return VkShaderStageFlags.TaskNV;

                case ShaderStage.MeshNV:
                    return VkShaderStageFlags.MeshNV;

                default:
                    return 0;

            }
        }



        internal static ShaderKind StageToShaderKind(this ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return ShaderKind.VertexShader;

                case ShaderStage.Fragment:
                    return ShaderKind.FragmentShader;

                case ShaderStage.Compute:
                    return ShaderKind.ComputeShader;

                case ShaderStage.Geometry:
                    return ShaderKind.GeometryShader;

                case ShaderStage.TessellationControl:
                    return ShaderKind.TessControlShader;

                case ShaderStage.TessellationEvaluation:
                    return ShaderKind.TessEvaluationShader;

                case ShaderStage.Raygen:
                    return ShaderKind.RaygenShader;

                case ShaderStage.AnyHit:
                    return ShaderKind.AnyhitShader;

                case ShaderStage.ClosestHit:
                    return ShaderKind.ClosesthitShader;

                case ShaderStage.Miss:
                    return ShaderKind.MissShader;

                case ShaderStage.Intersection:
                    return ShaderKind.IntersectionShader;

                case ShaderStage.Callable:
                    return ShaderKind.CallableShader;

                case ShaderStage.TaskNV:
                    return ShaderKind.TaskShader;

                case ShaderStage.MeshNV:
                    return ShaderKind.MeshShader;

                default:
                    return 0;

            }
        }

        public static VkSampleCountFlags ExtractMaxSampleCount(VkPhysicalDeviceProperties physical)
        {
            VkSampleCountFlags counts = physical.limits.framebufferColorSampleCounts & physical.limits.framebufferDepthSampleCounts;

            if ((counts & VkSampleCountFlags.Count64) != 0)
                return VkSampleCountFlags.Count64;

            if ((counts & VkSampleCountFlags.Count32) != 0)
                return VkSampleCountFlags.Count32;

            if ((counts & VkSampleCountFlags.Count16) != 0)
                return VkSampleCountFlags.Count16;

            if ((counts & VkSampleCountFlags.Count8) != 0)
                return VkSampleCountFlags.Count8;

            if ((counts & VkSampleCountFlags.Count4) != 0)
                return VkSampleCountFlags.Count4;

            if ((counts & VkSampleCountFlags.Count2) != 0)
                return VkSampleCountFlags.Count2;

            return VkSampleCountFlags.Count1;
        }


        internal static readonly Dictionary<uint, string> VendorNames = new Dictionary<uint, string>
        {
            [0x1002] = "AMD",
            [0x1010] = "ImgTec",
            [0x10DE] = "NVIDIA",
            [0x13B5] = "ARM",
            [0x5143] = "Qualcomm",
            [0x8086] = "INTEL",
        };

        public static void ImGuiAddFontFromMemoryTTF(string path = "ARIAL.TTF")
        {
            byte[] bytes = File.ReadAllBytes(path);
            fixed (byte* ptr = bytes)
            {
                //ImGui.GetIO().Fonts.AddFontFromMemoryTTF((IntPtr)ptr, 32, 15);
            }
        }


        public static int Size(this VertexType element)
        {
            switch (element)
            {
                case VertexType.Position: 
                case VertexType.Normal: 
                case VertexType.Color: 
                    return Interop.SizeOf<Vector3>();

                case VertexType.TextureCoordinate: 
                    return Interop.SizeOf<Vector2>();

                default: return 0;
            }
        }


        public static VkFormat ToPixelFormat(this VertexType element)
        {
            switch (element)
            {
                case VertexType.Position: 
                    return VkFormat.R32G32B32SFloat;

                case VertexType.Normal: 
                    return VkFormat.R32G32B32SFloat;

                case VertexType.TextureCoordinate: 
                    return VkFormat.R32G32SFloat;

                case VertexType.Color: 
                    return VkFormat.R32G32B32SFloat;

                default: return 0;
            }
        }

    }
}
