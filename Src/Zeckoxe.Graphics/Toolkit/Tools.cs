// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


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



        public static TDelegate GetInstanceProcAddr<TDelegate>(this GraphicsAdapter instance, string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetInstanceProcAddr(instance.instance, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }


        public static TDelegate GetDeviceProcAddr<TDelegate>(this GraphicsDevice device, string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetDeviceProcAddr(device.handle, Interop.String.ToPointer(name));

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
