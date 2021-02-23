// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Tools.cs
=============================================================================*/



using Vortice.ShaderCompiler;
using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;
using SharpSPIRVCross;

namespace Zeckoxe.Vulkan
{
    public static unsafe class Tools
    {
        public static string ExtractVersion(uint _value)
        {

            uint major = _value >> 22;
            uint minor = (_value >> 12) & 0x03FF;
            uint patch = _value & 0x0FFF;

            return $"{major}.{minor}.{patch}";
        }



        

        internal static VkDescriptorType StageTVkDescriptorType(this ResourceType stage)
        {
            switch (stage)
            {
                case ResourceType.UniformBuffer:
                    return VkDescriptorType.UniformBuffer;

                case ResourceType.StorageBuffer:
                    return VkDescriptorType.StorageBuffer;


                case ResourceType.StorageImage:
                    return VkDescriptorType.StorageImage;

                case ResourceType.SampledImage:
                    return VkDescriptorType.CombinedImageSampler;

                case ResourceType.SeparateImage:
                    return VkDescriptorType.SampledImage;

                case ResourceType.SeparateSamplers:
                    return VkDescriptorType.Sampler;

                case ResourceType.SubpassInput:
                    return VkDescriptorType.InputAttachment;

                case ResourceType.AccelerationStructure:
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

                case ShaderStage.RaygenKHR:
                    return VkShaderStageFlags.RaygenKHR;

                case ShaderStage.AnyHitKHR:
                    return VkShaderStageFlags.AnyHitKHR;

                case ShaderStage.ClosestHitKHR:
                    return VkShaderStageFlags.ClosestHitKHR;

                case ShaderStage.MissKHR:
                    return VkShaderStageFlags.MissKHR;

                case ShaderStage.IntersectionKHR:
                    return VkShaderStageFlags.IntersectionKHR;

                case ShaderStage.CallableKHR:
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

                case ShaderStage.RaygenKHR:
                    return ShaderKind.RaygenShader;

                case ShaderStage.AnyHitKHR:
                    return ShaderKind.AnyhitShader;

                case ShaderStage.ClosestHitKHR:
                    return ShaderKind.ClosesthitShader;

                case ShaderStage.MissKHR:
                    return ShaderKind.MissShader;

                case ShaderStage.IntersectionKHR:
                    return ShaderKind.IntersectionShader;

                case ShaderStage.CallableKHR:
                    return ShaderKind.CallableShader;

                case ShaderStage.TaskNV:
                    return ShaderKind.TaskShader;

                case ShaderStage.MeshNV:
                    return ShaderKind.MeshShader;

                default:
                    return 0;

            }
        }

        public static VkSampleCountFlags ExtractMaxSampleCount(VkPhysicalDeviceProperties physicalDeviceProperties)
        {
            VkSampleCountFlags counts = physicalDeviceProperties.limits.framebufferColorSampleCounts & physicalDeviceProperties.limits.framebufferDepthSampleCounts;

            if ((counts & VkSampleCountFlags.Count64) != 0)
            {
                return VkSampleCountFlags.Count64;
            }

            if ((counts & VkSampleCountFlags.Count32) != 0)
            {
                return VkSampleCountFlags.Count32;
            }

            if ((counts & VkSampleCountFlags.Count16) != 0)
            {
                return VkSampleCountFlags.Count16;
            }

            if ((counts & VkSampleCountFlags.Count8) != 0)
            {
                return VkSampleCountFlags.Count8;
            }

            if ((counts & VkSampleCountFlags.Count4) != 0)
            {
                return VkSampleCountFlags.Count4;
            }

            if ((counts & VkSampleCountFlags.Count2) != 0)
            {
                return VkSampleCountFlags.Count2;
            }

            return VkSampleCountFlags.Count1;
        }


        private static readonly Dictionary<uint, string> VendorNames = new Dictionary<uint, string>
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
    }
}
