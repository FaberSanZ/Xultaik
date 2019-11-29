// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsInstance.cs
=============================================================================*/


using System;
using System.Collections.Generic;
//using Zeckoxe.Collections;
using System.Text;
using Vulkan;
using System.Linq;
using Zeckoxe.Core;
using System.Runtime.InteropServices;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsInstance
    {
        public List<string> InstanceExtensions { get; private set; } = new List<string>();
        public List<string> EnumerateInstanceExtensions { get; private set; } = new List<string>();

        public PresentationParameters Parameters { get; set; }



        internal VkInstance NativeInstance { get; private set; }

        public GraphicsInstance(PresentationParameters parameters)
        {
            Parameters = parameters;

            NativeInstance = CreateInstance();
        }


        public VkInstance CreateInstance()
        {

            VkApplicationInfo AppInfo = new VkApplicationInfo()
            {
                sType = VkStructureType.ApplicationInfo,
                apiVersion = new Version(1, 0, 0),
                applicationVersion = new Version(0, 0, 1),
                engineVersion = new Version(0, 0, 2),
                pApplicationName = Interop.String.ToPointer("Zeckoxe Engine"),
                pEngineName = Interop.String.ToPointer("Zeckoxe"),
            };


            if (Parameters.Settings.Validation)
            {
                // TODO:
            }

            EnumerateInstanceExtensions = Tools.EnumerateInstanceExtensions();

            if (EnumerateInstanceExtensions.Contains("VK_KHR_surface"))
                InstanceExtensions.Add("VK_KHR_surface");


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (EnumerateInstanceExtensions.Contains("VK_KHR_win32_surface"))
                    InstanceExtensions.Add("VK_KHR_win32_surface");
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (EnumerateInstanceExtensions.Contains("VK_MVK_macos_surface"))
                    InstanceExtensions.Add("VK_MVK_macos_surface");


                if (EnumerateInstanceExtensions.Contains("VK_MVK_ios_surface"))
                    InstanceExtensions.Add("VK_MVK_ios_surface");
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (EnumerateInstanceExtensions.Contains("VK_KHR_android_surface"))
                    InstanceExtensions.Add("VK_KHR_android_surface");


                if (EnumerateInstanceExtensions.Contains("VK_KHR_xlib_surface"))
                    InstanceExtensions.Add("VK_KHR_xlib_surface");


                if (EnumerateInstanceExtensions.Contains("VK_KHR_wayland_surface"))
                    InstanceExtensions.Add("VK_KHR_wayland_surface");
            }




            VkInstanceCreateInfo instanceCreateInfo = new VkInstanceCreateInfo()
            {
                sType = VkStructureType.InstanceCreateInfo,
                pNext = null,
                pApplicationInfo = &AppInfo,
                enabledExtensionCount = (uint)InstanceExtensions.Count(),
                ppEnabledExtensionNames = Interop.String.AllocToPointers(InstanceExtensions.ToArray()),
            };


            VkInstance instance;
            VulkanNative.vkCreateInstance(&instanceCreateInfo, (VkAllocationCallbacks*)null, &instance);
            return instance;
        }
        
    }
   
}
