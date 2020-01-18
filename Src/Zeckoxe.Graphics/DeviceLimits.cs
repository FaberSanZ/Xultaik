// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	DeviceLimits.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vulkan;

namespace Zeckoxe.Graphics
{
    public class DeviceLimits : GraphicsResource
    {
        public int VULKAN_NUM_DESCRIPTOR_SETS = 8;
        public int VULKAN_NUM_BINDINGS = 16;
        public int VULKAN_NUM_BINDINGS_BINDLESS_VARYING = 64 * 1024;
        public int VULKAN_NUM_BINDINGS_BINDLESS = 4 * 1024;
        public int VULKAN_NUM_ATTACHMENTS = 8;
        public int VULKAN_NUM_VERTEX_ATTRIBS = 16;
        public int VULKAN_NUM_VERTEX_BUFFERS = 4;
        public int VULKAN_PUSH_CONSTANT_SIZE = 128;
        public int VULKAN_MAX_UBO_SIZE = 16 * 1024;
        public int VULKAN_NUM_SPEC_CONSTANTS = 8;


        public float TimestampPeriod { get; internal set; }

        public MultisampleCount MultisampleCount { get; internal set; }

        public DeviceLimits(GraphicsDevice device) : base(device)
        {
            Recreate();
        }

        private void Recreate()
        {
            TimestampPeriod = NativeDevice.NativeAdapter.Properties.limits.maxBoundDescriptorSets;
            //MultisampleCount = VulkanConvert.ToSampleCountFlags_Invert(ExtractMaxSampleCount(NativeDevice.NativeAdapter.Properties));

        }



        public VkSampleCountFlags ExtractMaxSampleCount(VkPhysicalDeviceProperties physicalDeviceProperties)
        {
            VkSampleCountFlags counts = physicalDeviceProperties.limits.framebufferColorSampleCounts & physicalDeviceProperties.limits.framebufferDepthSampleCounts;

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

        //public string ExtractVersion(uint _value)
        //{

        //    uint major = _value >> 22;
        //    uint minor = (_value >> 12) & 0x03FF;
        //    uint patch = _value & 0x0FFF;

        //    return $"{major}.{minor}.{patch}";
        //}
    }
}
