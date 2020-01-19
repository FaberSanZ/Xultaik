// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Tools.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vulkan;
using Zeckoxe.Core;
using static Vulkan.VulkanNative;

namespace Zeckoxe.Graphics
{
    public static unsafe class Tools
    {
        public static List<string> EnumerateInstanceExtensions()
        {
            uint Count = 0;

            vkEnumerateInstanceExtensionProperties((byte*)null, ref Count, null);
            VkExtensionProperties* ext = stackalloc VkExtensionProperties[(int)Count];
            vkEnumerateInstanceExtensionProperties((byte*)null, ref Count, ext);


            List<string> extensionName = new List<string>();
            for (int i = 0; i < Count; i++)
                extensionName.Add(Interop.String.FromPointer(ext[i].extensionName));

            return extensionName;
        }

        internal static VkSampleCountFlags ExtractMaxSampleCount(VkPhysicalDeviceProperties physicalDeviceProperties)
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

        public static int BlockSizeInBytes(PixelFormat format)
        {
            return 1;
        }

        public static void ImGuiAddFontFromMemoryTTF(string path = "ARIAL.TTF")
        {
            var bytes = File.ReadAllBytes(path);
            //fixed (void* ptr = bytes)
            //    ImGui.GetIO().Fonts.AddFontFromMemoryTTF(new IntPtr(ptr), 32, 15);
        }
    }
}
