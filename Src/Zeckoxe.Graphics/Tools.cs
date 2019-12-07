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
