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
                case VertexType.Position: return Interop.SizeOf<Vector3>();

                case VertexType.Normal: return 12;

                case VertexType.TextureCoordinate: return 8;

                case VertexType.Color: return 12;


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
