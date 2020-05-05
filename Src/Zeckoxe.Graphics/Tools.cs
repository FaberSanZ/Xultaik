// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Tools.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.IO;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public static unsafe class Tools
    {
        public static List<string> EnumerateInstanceExtensions()
        {
            uint count = 0;

            vkEnumerateInstanceExtensionProperties((byte*)null, &count, null);
            VkExtensionProperties* ext = stackalloc VkExtensionProperties[(int)count];
            vkEnumerateInstanceExtensionProperties((byte*)null, &count, ext);


            List<string> extensionName = new List<string>();
            for (int i = 0; i < count; i++)
            {
                extensionName.Add(Interop.String.FromPointer(ext[i].extensionName));
            }

            return extensionName;
        }


        public static PixelFormat GetSupportedDepthFormat(this GraphicsAdapter adapter, List<PixelFormat> depthFormats)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format

            PixelFormat depthFormat = PixelFormat.Undefined;

            foreach (PixelFormat format in depthFormats)
            {
                vkGetPhysicalDeviceFormatProperties(adapter.NativePhysicalDevice, (VkFormat)format, out VkFormatProperties formatProps);

                // Format must support depth stencil attachment for optimal tiling
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) != 0)
                {
                    depthFormat = format;
                }
            }

            

            return depthFormat;
        }


        public static TDelegate GetInstanceProcAddr<TDelegate>(this GraphicsInstance instance, string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetInstanceProcAddr(instance.NativeInstance, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }


        public static TDelegate GetDeviceProcAddr<TDelegate>(this GraphicsDevice device, string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetDeviceProcAddr(device.Device, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }

        internal static VkSampleCountFlags ExtractMaxSampleCount(VkPhysicalDeviceProperties physicalDeviceProperties)
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

        public static int BlockSizeInBytes(PixelFormat format)
        {
            return 1;
        }

        public static void ImGuiAddFontFromMemoryTTF(string path = "ARIAL.TTF")
        {
            byte[] bytes = File.ReadAllBytes(path);
            fixed (byte* ptr = bytes)
            {
                //ImGui.GetIO().Fonts.AddFontFromMemoryTTF(new IntPtr(ptr), 32, 15);
            }
        }
    }
}
