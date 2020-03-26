// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsInstance.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
//using Zeckoxe.Collections;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsInstance : IDisposable
    {
        public List<string> InstanceExtensions { get; private set; } = new List<string>();
        public List<string> EnumerateInstanceExtensions { get; private set; } = new List<string>();
        public List<string> ValidationLayer { get; private set; } = new List<string>();


        public PresentationParameters Parameters { get; set; }



        internal VkInstance NativeInstance { get; private set; }

        internal VkDebugReportCallbackEXT _debugReportCallbackHandle; public unsafe delegate uint PFN_vkDebugReportCallbackEXT(
     uint flags,
     VkDebugReportObjectTypeEXT objectType,
     ulong @object,
     UIntPtr location,
     int messageCode,
     byte* pLayerPrefix,
     byte* pMessage,
     void* pUserData);
        internal PFN_vkDebugReportCallbackEXT _debugCallbackFunc;



        public GraphicsInstance(PresentationParameters parameters)
        {
            Parameters = parameters;
            vkInitialize();
            NativeInstance = CreateInstance();
            vkLoadInstance(NativeInstance);

            if (Parameters.Settings.Validation)
            {
                // TODO: VkDebugReportFlagsEXT 
                VkDebugReportFlagsEXT flags =
                    VkDebugReportFlagsEXT.WarningEXT |
                    VkDebugReportFlagsEXT.ErrorEXT |
                    VkDebugReportFlagsEXT.DebugEXT |
                    VkDebugReportFlagsEXT.PerformanceWarningEXT |
                    VkDebugReportFlagsEXT.InformationEXT;

                CreateDebugReportCallback(flags);
            }

        }


        public VkInstance CreateInstance()
        {

            VkApplicationInfo AppInfo = new VkApplicationInfo()
            {
                sType = VkStructureType.ApplicationInfo,
                apiVersion = new VkVersion(1, 0, 0),
                applicationVersion = new VkVersion(0, 0, 1),
                engineVersion = new VkVersion(0, 0, 2),
                pApplicationName = Interop.String.ToPointer("Zeckoxe Engine"),
                pEngineName = Interop.String.ToPointer("Zeckoxe"),
            };


            EnumerateInstanceExtensions = Tools.EnumerateInstanceExtensions();

            if (EnumerateInstanceExtensions.Contains("VK_KHR_surface"))
            {
                InstanceExtensions.Add("VK_KHR_surface");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (EnumerateInstanceExtensions.Contains("VK_KHR_win32_surface"))
                {
                    InstanceExtensions.Add("VK_KHR_win32_surface");
                }
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (EnumerateInstanceExtensions.Contains("VK_MVK_macos_surface"))
                {
                    InstanceExtensions.Add("VK_MVK_macos_surface");
                }

                if (EnumerateInstanceExtensions.Contains("VK_MVK_ios_surface"))
                {
                    InstanceExtensions.Add("VK_MVK_ios_surface");
                }
            }


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (EnumerateInstanceExtensions.Contains("VK_KHR_android_surface"))
                {
                    InstanceExtensions.Add("VK_KHR_android_surface");
                }

                if (EnumerateInstanceExtensions.Contains("VK_KHR_xlib_surface"))
                {
                    InstanceExtensions.Add("VK_KHR_xlib_surface");
                }

                if (EnumerateInstanceExtensions.Contains("VK_KHR_wayland_surface"))
                {
                    InstanceExtensions.Add("VK_KHR_wayland_surface");
                }
            }


            if (Parameters.Settings.Validation)
            {
                // TODO: Validation complete?
                ValidationLayer.Add("VK_LAYER_LUNARG_standard_validation");
                InstanceExtensions.Add("VK_EXT_debug_report");
            }


            VkInstanceCreateInfo instanceCreateInfo = new VkInstanceCreateInfo()
            {
                sType = VkStructureType.InstanceCreateInfo,
                pNext = null,
                pApplicationInfo = &AppInfo,
                enabledExtensionCount = (uint)InstanceExtensions.Count(),
                ppEnabledExtensionNames = (byte*)Interop.String.AllocToPointers(InstanceExtensions.ToArray()),
                enabledLayerCount = (uint)ValidationLayer.Count(),
                ppEnabledLayerNames = (byte*)Interop.String.AllocToPointers(ValidationLayer.ToArray())
            };


            VkInstance instance;
            vkCreateInstance(&instanceCreateInfo, (VkAllocationCallbacks*)null, &instance);
            return instance;
        }




        private unsafe uint DebugCallback(
            uint flags,
            VkDebugReportObjectTypeEXT objectType,
            ulong @object,
            UIntPtr location,
            int messageCode,
            byte* pLayerPrefix,
            byte* pMessage,
            void* pUserData)
        {
            Console.WriteLine("{0}     ", Interop.String.FromPointer(pLayerPrefix));
            Console.WriteLine("     ");
            Console.WriteLine("     ");
            Console.WriteLine("{0}     ", Interop.String.FromPointer(pMessage));

            return 0;
        }

        internal unsafe delegate VkResult vkCreateDebugReportCallbackEXT_d(
        VkInstance instance,
        VkDebugReportCallbackCreateInfoEXT* createInfo,
        IntPtr allocatorPtr,
        out VkDebugReportCallbackEXT ret);

        internal unsafe delegate void vkDestroyDebugReportCallbackEXT_d(
            VkInstance instance,
            VkDebugReportCallbackEXT callback,
            VkAllocationCallbacks* pAllocator);

        private unsafe VkResult CreateDebugReportCallback(VkDebugReportFlagsEXT flags)
        {
            _debugCallbackFunc = DebugCallback;
            IntPtr debugFunctionPtr = Interop.GetFunctionPointerForDelegate(_debugCallbackFunc);
            VkDebugReportCallbackCreateInfoEXT debugCallbackInfo = new VkDebugReportCallbackCreateInfoEXT()
            {
                sType = VkStructureType.DebugReportCallbackCreateInfoEXT,
                pNext = (void*)null,
                flags = flags,
                pfnCallback = debugFunctionPtr,
            };

            var vkCreateDebugReportCallbackEXT = this.GetInstanceProcAddr<vkCreateDebugReportCallbackEXT_d>("vkCreateDebugReportCallbackEXT");

            if (vkCreateDebugReportCallbackEXT == null)
            {
                return VkResult.ErrorValidationFailedEXT;
            }

            return vkCreateDebugReportCallbackEXT(NativeInstance, &debugCallbackInfo, IntPtr.Zero, out _debugReportCallbackHandle);
        }

        private unsafe void DestroyDebugReportCallback()
        {
            //_debugCallbackFunc = null;
            vkDestroyDebugReportCallbackEXT_d vkDestroyDebugReportCallbackEXT = this.GetInstanceProcAddr<vkDestroyDebugReportCallbackEXT_d>("vkDestroyDebugReportCallbackEXT");
            vkDestroyDebugReportCallbackEXT(NativeInstance, _debugReportCallbackHandle, null);
        }

        public void Dispose()
        {
            DestroyDebugReportCallback();
        }
    }

}
