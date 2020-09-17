// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	GraphicsAdapter.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsAdapter : IDisposable
    {
        internal VkInstance instance;

        internal vkDebugUtilsMessengerCallbackEXT _debugMessengerCallbackFunc;
        internal VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;

        internal uint instance_extensions_count;
        internal uint device_count; // number of GPUs we're rendering to --- if DG is disabled, this is 1

        internal VkPhysicalDevice NativePhysicalDevice;
        internal VkPhysicalDevice[] NativePhysicalDevices;
        internal VkPhysicalDeviceProperties Properties;
        internal VkPhysicalDeviceFeatures2 Features2;
        internal VkPhysicalDeviceRayTracingFeaturesKHR DeviceRayTracingFeatures;
        private VkPhysicalDeviceSubgroupProperties subgroup_properties;
        private VkPhysicalDevice8BitStorageFeatures storage_8bit_features;
        private VkPhysicalDevice16BitStorageFeatures storage_16bit_features;
        private VkPhysicalDeviceShaderFloat16Int8Features float16_int8_features;
        private VkPhysicalDeviceFeatures enabled_features;
        private VkPhysicalDeviceExternalMemoryHostPropertiesEXT host_memory_properties;
        private VkPhysicalDeviceMultiviewFeatures multiview_features;
        private VkPhysicalDeviceImagelessFramebufferFeatures imageless_features;
        private VkPhysicalDeviceSubgroupSizeControlFeaturesEXT subgroup_size_control_features;
        private VkPhysicalDeviceSubgroupSizeControlPropertiesEXT subgroup_size_control_properties;
        private VkPhysicalDeviceComputeShaderDerivativesFeaturesNV compute_shader_derivative_features;
        private VkPhysicalDeviceHostQueryResetFeatures host_query_reset_features;
        private VkPhysicalDeviceShaderDemoteToHelperInvocationFeaturesEXT demote_to_helper_invocation_features;
        private VkPhysicalDeviceScalarBlockLayoutFeatures scalar_block_features;
        private VkPhysicalDeviceUniformBufferStandardLayoutFeatures ubo_std430_features;
        private VkPhysicalDeviceTimelineSemaphoreFeatures timeline_semaphore_features;
        private VkPhysicalDeviceDescriptorIndexingFeatures descriptor_indexing_features;
        private VkPhysicalDeviceDescriptorIndexingProperties descriptor_indexing_properties;
        private VkPhysicalDeviceConservativeRasterizationPropertiesEXT conservative_rasterization_properties;
        private VkPhysicalDevicePerformanceQueryFeaturesKHR performance_query_features;
        private VkPhysicalDeviceSamplerYcbcrConversionFeatures sampler_ycbcr_conversion_features;
        private VkPhysicalDeviceDriverProperties driver_properties;




        public GraphicsAdapter(PresentationParameters parameters)
        {
            Parameters = parameters;
            Log = new ConsoleLog();
            InstanceExtensionsNames = new List<string>();

            Recreate();
        }



        public PresentationParameters Parameters { get; set; }

        public List<string> InstanceExtensionsNames;

        //public List<string> EnumerateInstanceExtensions;

        //public List<string> ValidationLayer;

        public ILog Log;
        
        public bool SupportsPhysicalDeviceProperties2;
        public bool SupportsSurface;
        private readonly bool supports_external = false;
        private readonly bool supports_dedicated = false;
        private readonly bool supports_image_format_list = false;
        private readonly bool supports_debug_marker = false;
        private readonly bool supports_debug_utils = false;
        private readonly bool supports_mirror_clamp_to_edge = false;
        private readonly bool supports_google_display_timing = false;
        private readonly bool supports_nv_device_diagnostic_checkpoints = false;
        private readonly bool supports_vulkan_11_instance = false;
        private readonly bool supports_vulkan_11_device = false;
        private readonly bool supports_external_memory_host = false;
        private readonly bool supports_surface_capabilities2 = false;
        private readonly bool supports_full_screen_exclusive = false;
        private readonly bool supports_update_template = false;
        private readonly bool supports_maintenance_1 = false;
        private readonly bool supports_maintenance_2 = false;
        private readonly bool supports_maintenance_3 = false;
        private readonly bool supports_descriptor_indexing = false;
        private readonly bool supports_conservative_rasterization = false;
        private readonly bool supports_bind_memory2 = false;
        private readonly bool supports_get_memory_requirements2 = false;
        private readonly bool supports_draw_indirect_count = false;
        private readonly bool supports_draw_parameters = false;
        private readonly bool supports_driver_properties = false;
        private readonly bool supports_calibrated_timestamps = false;


        public DeviceType DeviceType => (DeviceType)Properties.deviceType;

        public uint VendorId => Properties.vendorID;

        public bool RayTracingSupport => DeviceRayTracingFeatures.rayTracing;

        public string DeviceName
        {
            get
            {
                VkPhysicalDeviceProperties properties = Properties;
                return Interop.String.FromPointer(properties.deviceName);
            }
        }

        public string Description
        {
            get
            {
                VkPhysicalDeviceProperties properties = Properties;
                return Interop.String.FromPointer(properties.deviceName) + $" - {VendorNameString(VendorId)}";
            }
        }



        public void Recreate()
        {
            vkInitialize().CheckResult();

            SupportsExtensions();

            CreateInstance(InstanceExtensionsNames.ToArray());

            CreatePhysicalDevice();


            Properties = GetProperties();
            Features2 = GetPhysicalDeviceFeatures2();
            //SupportsPhysicalDeviceProperties2 = true;
            DeviceRayTracingFeatures = GetPhysicalDeviceFeaturesRayTracing();

            //Features2 = new VkPhysicalDeviceFeatures2();
        }


        internal void SupportsExtensions()
        {
            IEnumerable<string> instance_extensions_names = Instance_Extensions()
                                                    .ToArray()
                                                    .Select(m => Interop.String.FromPointer(m.extensionName));


            if (Parameters.Settings.Validation && instance_extensions_names.Contains("VK_EXT_debug_report"))
            {
                InstanceExtensionsNames.Add("VK_EXT_debug_report");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (instance_extensions_names.Contains("VK_KHR_win32_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_win32_surface");
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (instance_extensions_names.Contains("VK_MVK_macos_surface"))
                {
                    InstanceExtensionsNames.Add("VK_MVK_macos_surface");
                }

                if (instance_extensions_names.Contains("VK_MVK_ios_surface"))
                {
                    InstanceExtensionsNames.Add("VK_MVK_ios_surface");
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (instance_extensions_names.Contains("VK_KHR_android_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_android_surface");
                }

                if (instance_extensions_names.Contains("VK_KHR_xlib_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_xlib_surface");
                }

                if (instance_extensions_names.Contains("VK_KHR_wayland_surface"))
                {
                    InstanceExtensionsNames.Add("VK_KHR_wayland_surface");
                }
            }

            if (instance_extensions_names.Contains("VK_KHR_surface"))
            {
                InstanceExtensionsNames.Add("VK_KHR_surface");
                SupportsSurface = true;
            }

            if (instance_extensions_names.Contains("VK_KHR_get_physical_device_properties2"))
            {
                InstanceExtensionsNames.Add("VK_KHR_get_physical_device_properties2");
                SupportsPhysicalDeviceProperties2 = true;
            }
        }

        internal void CreateInstance(string[] extensions)
        {

            VkApplicationInfo app_info = new VkApplicationInfo
            {
                sType = VkStructureType.ApplicationInfo,
                pNext = null,
                apiVersion = new VkVersion(1, 0, 0),
                applicationVersion = new VkVersion(0, 0, 1),
                engineVersion = new VkVersion(0, 0, 2),
                pApplicationName = Interop.String.ToPointer("Zeckoxe Engine"),
                pEngineName = Interop.String.ToPointer("Zeckoxe"),
            };

            string[] layers = new string[]
            {
                "VK_LAYER_KHRONOS_validation",
            };

            VkInstanceCreateInfo inst_info = new VkInstanceCreateInfo
            {
                sType = VkStructureType.InstanceCreateInfo,
                pNext = null,
                flags = VkInstanceCreateFlags.None,
                pApplicationInfo = &app_info,
                ppEnabledExtensionNames = Interop.String.AllocToPointers(extensions),
                enabledExtensionCount = (uint)extensions.Length,
            };


            VkDebugUtilsMessengerCreateInfoEXT debugUtilsCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
            {
                sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT,
                pNext = null,
                flags = VkDebugUtilsMessengerCreateFlagsEXT.None,
                pUserData = null,
                messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.ErrorEXT | VkDebugUtilsMessageSeverityFlagsEXT.WarningEXT | VkDebugUtilsMessageSeverityFlagsEXT.InfoEXT,
                messageType = VkDebugUtilsMessageTypeFlagsEXT.ValidationEXT | VkDebugUtilsMessageTypeFlagsEXT.PerformanceEXT,
                pfnUserCallback = Interop.GetFunctionPointerForDelegate(_debugMessengerCallbackFunc = DebugMessengerCallback),
            };

            if (Parameters.Settings.Validation)
            {
                inst_info.pNext = &debugUtilsCreateInfo;
                inst_info.ppEnabledLayerNames = Interop.String.AllocToPointers(layers);
                inst_info.enabledLayerCount = (uint)layers.Length;

            }

            vkCreateInstance(&inst_info, null, out instance);
            vkLoadInstance(instance);

            //vkCreateDebugUtilsMessengerEXT(instance, &debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
        }



        internal void CreatePhysicalDevice()
        {

            // Physical Device
            uint count = 0;
            vkEnumeratePhysicalDevices(instance, &count, null);
            device_count = count;

            VkPhysicalDevice[] physicalDevices = new VkPhysicalDevice[(int)count];
            VkPhysicalDevice* physicalDevicesptr = stackalloc VkPhysicalDevice[(int)count];

            if (device_count is 1)
            {
                vkEnumeratePhysicalDevices(instance, &count, physicalDevicesptr);
            }
            else
            {
                fixed (VkPhysicalDevice* ptr = physicalDevices)
                {
                    vkEnumeratePhysicalDevices(instance, &count, ptr);
                }
            }

            NativePhysicalDevice = physicalDevicesptr[0];
            NativePhysicalDevices = device_count is 1 ? new VkPhysicalDevice[] { physicalDevicesptr[0] } : physicalDevices;

        }


        internal unsafe ReadOnlySpan<VkExtensionProperties> Instance_Extensions()
        {
            uint count = 0;
            vkEnumerateInstanceExtensionProperties(null, &count, null).CheckResult();

            ReadOnlySpan<VkExtensionProperties> properties = new VkExtensionProperties[count];
            fixed (VkExtensionProperties* ptr = properties)
            {
                vkEnumerateInstanceExtensionProperties(null, &count, ptr).CheckResult();
            }

            return properties;
        }



        internal VkExtensionProperties* Instance_ExtensionsPtr()
        {
            uint count = 0;

            vkEnumerateInstanceExtensionProperties(null, &count, null).CheckResult();
            instance_extensions_count = count;

            VkExtensionProperties* ext = stackalloc VkExtensionProperties[(int)count];
            vkEnumerateInstanceExtensionProperties(null, &count, ext).CheckResult();
            return ext;
        }


        public PixelFormat GetSupportedDepthFormat(IEnumerable<PixelFormat> depthFormats)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format

            PixelFormat depthFormat = PixelFormat.Undefined;

            foreach (PixelFormat format in depthFormats)
            {
                vkGetPhysicalDeviceFormatProperties(NativePhysicalDevice, (VkFormat)format, out VkFormatProperties formatProps);

                // Format must support depth stencil attachment for optimal tiling
                if ((formatProps.optimalTilingFeatures & VkFormatFeatureFlags.DepthStencilAttachment) != 0)
                {
                    depthFormat = format;
                }
            }



            return depthFormat;
        }



        internal VkBool32 DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, IntPtr userData)
        {
            string message = Interop.String.FromPointer(pCallbackData->pMessage);

            if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.ValidationEXT)
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.ErrorEXT)
                {
                    Log.Error("Vulkan", $"Validation: {messageSeverity} - {message}");

                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.WarningEXT)
                {
                    Log.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
                }

            }
            else
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.ErrorEXT)
                {
                    Log.Error("Vulkan", $"[Vulkan]: {messageSeverity} - {message}");
                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.WarningEXT)
                {
                    Log.Warn($"[Vulkan]: {messageSeverity} - {message}");
                }

                //Log.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
            }

            return VkBool32.False;
        }


        internal VkPhysicalDeviceRayTracingFeaturesKHR GetPhysicalDeviceFeaturesRayTracing()
        {
            VkPhysicalDeviceRayTracingFeaturesKHR rayTracingFeatures = new VkPhysicalDeviceRayTracingFeaturesKHR()
            {
                sType = VkStructureType.PhysicalDeviceRayTracingFeaturesKHR,
                pNext = null,
            };

            VkPhysicalDeviceFeatures2 deviceFeatures2 = new VkPhysicalDeviceFeatures2()
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
                pNext = &rayTracingFeatures,
            };


            vkGetPhysicalDeviceFeatures2(NativePhysicalDevice, out deviceFeatures2);

            return rayTracingFeatures;
        }



        internal VkPhysicalDeviceFeatures2 GetPhysicalDeviceFeatures2()
        {
            VkPhysicalDeviceRayTracingFeaturesKHR rayTracingFeatures = new VkPhysicalDeviceRayTracingFeaturesKHR()
            {
                sType = VkStructureType.PhysicalDeviceRayTracingFeaturesKHR,
                pNext = null,
            };

            VkPhysicalDeviceFeatures2 deviceFeatures2 = new VkPhysicalDeviceFeatures2()
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
                pNext = &rayTracingFeatures,
            };


            vkGetPhysicalDeviceFeatures2(NativePhysicalDevice, out deviceFeatures2);

            return deviceFeatures2;
        }

        internal VkPhysicalDeviceProperties GetProperties()
        {
            vkGetPhysicalDeviceProperties(NativePhysicalDevice, out VkPhysicalDeviceProperties physicalDeviceProperties);
            return physicalDeviceProperties;
        }




        internal string VendorNameString(uint vendorId)
        {
            switch (vendorId)
            {
                case 0x1002:
                    return "AMD";

                case 0x1010:
                    return "ImgTec";

                case 0x10DE:
                    return "NVIDIA";

                case 0x13B5:
                    return "ARM";

                case 0x5143:
                    return "Qualcomm";

                case 0x8086:
                    return "Intel";

                default:
                    return "Unknown";
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
