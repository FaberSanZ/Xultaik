// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

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

        internal VkPhysicalDevice handle;
        internal VkPhysicalDevice[] handles;
        internal VkPhysicalDeviceProperties device_properties;
        internal VkPhysicalDeviceRayTracingFeaturesKHR DeviceRayTracingFeatures;
        internal VkPhysicalDeviceSubgroupProperties subgroup_properties;
        internal VkPhysicalDevice8BitStorageFeatures storage_8bit_features;
        internal VkPhysicalDevice16BitStorageFeatures storage_16bit_features;
        internal VkPhysicalDeviceShaderFloat16Int8Features float16_int8_features;
        internal VkPhysicalDeviceFeatures enabled_features;
        internal VkPhysicalDeviceExternalMemoryHostPropertiesEXT host_memory_properties;
        internal VkPhysicalDeviceMultiviewFeatures multiview_features;
        internal VkPhysicalDeviceImagelessFramebufferFeatures imageless_features;
        internal VkPhysicalDeviceSubgroupSizeControlFeaturesEXT subgroup_size_control_features;
        internal VkPhysicalDeviceSubgroupSizeControlPropertiesEXT subgroup_size_control_properties;
        internal VkPhysicalDeviceComputeShaderDerivativesFeaturesNV compute_shader_derivative_features;
        internal VkPhysicalDeviceHostQueryResetFeatures host_query_reset_features;
        internal VkPhysicalDeviceShaderDemoteToHelperInvocationFeaturesEXT demote_to_helper_invocation_features;
        internal VkPhysicalDeviceScalarBlockLayoutFeatures scalar_block_features;
        internal VkPhysicalDeviceUniformBufferStandardLayoutFeatures ubo_std430_features;
        internal VkPhysicalDeviceTimelineSemaphoreFeatures timeline_semaphore_features;
        internal VkPhysicalDeviceDescriptorIndexingFeatures descriptor_indexing_features;
        internal VkPhysicalDeviceDescriptorIndexingProperties descriptor_indexing_properties;
        internal VkPhysicalDeviceConservativeRasterizationPropertiesEXT conservative_rasterization_properties;
        internal VkPhysicalDevicePerformanceQueryFeaturesKHR performance_query_features;
        internal VkPhysicalDeviceSamplerYcbcrConversionFeatures sampler_ycbcr_conversion_features;
        internal VkPhysicalDeviceDriverProperties driver_properties;




        public GraphicsAdapter(PresentationParameters parameters)
        {
            Parameters = parameters;
            Log = new ConsoleLog();
            InstanceExtensionsNames = new List<string>();

            Recreate();
        }


        public Version EngineVersion { get; internal set; } = new Version(1, 2, 155);

        public PresentationParameters Parameters { get; set; }

        public List<string> InstanceExtensionsNames { get; private set; }

        public List<string> ValidationLayer { get; private set; }

        public ILog Log { get; set; }

        public DeviceType DeviceType => (DeviceType)device_properties.deviceType;

        public uint VendorId => device_properties.vendorID;

        public bool RayTracingSupport => DeviceRayTracingFeatures.rayTracing;

        public float TimestampPeriod => device_properties.limits.timestampPeriod;

        public uint MaxDrawIndirectCount => device_properties.limits.maxDrawIndirectCount;

        public MultisampleCount MultisampleCount => (MultisampleCount)Tools.ExtractMaxSampleCount(device_properties); // TODO: MultisampleCount.ToVkSampleCountFlags

        public bool SupportsPhysicalDeviceProperties2 { get; private set; }

        public bool SupportsSurface { get; private set; }

        public bool supports_external { get; private set; }

        public bool supports_dedicated { get; private set; }

        public bool supports_image_format_list { get; private set; }

        public bool supports_debug_marker { get; private set; }

        public bool supports_debug_utils { get; private set; }

        public bool supports_mirror_clamp_to_edge { get; private set; }

        public bool supports_google_display_timing { get; private set; }

        public bool supports_nv_device_diagnostic_checkpoints { get; private set; }

        public bool supports_vulkan_11_instance { get; private set; }

        public bool supports_vulkan_11_device { get; private set; }

        public bool supports_external_memory_host { get; private set; }

        public bool supports_surface_capabilities2 { get; private set; }

        public bool supports_full_screen_exclusive { get; private set; }

        public bool supports_update_template { get; private set; }

        public bool supports_maintenance_1 { get; private set; }

        public bool supports_maintenance_2 { get; private set; }

        public bool supports_maintenance_3 { get; private set; }

        public bool supports_descriptor_indexing { get; private set; }

        public bool supports_conservative_rasterization { get; private set; }

        public bool supports_bind_memory2 { get; private set; }

        public bool supports_get_memory_requirements2 { get; private set; }

        public bool supports_draw_indirect_count { get; private set; }

        public bool supports_draw_parameters { get; private set; }

        public bool supports_driver_properties { get; private set; }

        public bool supports_calibrated_timestamps { get; private set; }


        public string DeviceName
        {
            get
            {
                VkPhysicalDeviceProperties properties = device_properties;
                return Interop.String.FromPointer(properties.deviceName);
            }
        }

        public string Description
        {
            get
            {
                VkPhysicalDeviceProperties properties = device_properties;
                return Interop.String.FromPointer(properties.deviceName) + $" - {VendorNameString(VendorId)}";
            }
        }



        public void Recreate()
        {
            vkInitialize().CheckResult();

            SupportsExtensions();

            CreateInstance(InstanceExtensionsNames.ToArray());

            CreatePhysicalDevice();

            CreatePhysicalDeviceProperties();


            //Features2 = GetPhysicalDeviceFeatures2();
            //DeviceRayTracingFeatures = GetPhysicalDeviceFeaturesRayTracing();

            VkPhysicalDeviceFeatures2 features = new VkPhysicalDeviceFeatures2
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
            };

            storage_8bit_features = new VkPhysicalDevice8BitStorageFeatures
            {
                sType = VkStructureType.PhysicalDevice8bitStorageFeatures,
            };

            bool has_pdf2 = SupportsPhysicalDeviceProperties2 || (supports_vulkan_11_instance && supports_vulkan_11_device);


            void** ppNext = &features.pNext;

            if (has_pdf2)
            {
                if (/*VK_KHR_8BIT_STORAGE_EXTENSION_NAME*/true)
                {
                    VkPhysicalDevice8BitStorageFeatures feature = storage_8bit_features;
                    //enabled_extensions.push_back(VK_KHR_8BIT_STORAGE_EXTENSION_NAME);
                    *ppNext = &feature;
                    ppNext = &feature.pNext;
                }
            }
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
                messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Info,
                messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance,
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


            if (device_count is 0)
            {
                // TODO: physicalDevices is 0
                return;
            }
            else if (device_count is 1)
            {
                vkEnumeratePhysicalDevices(instance, &count, physicalDevicesptr);
            }
            else if (device_count > 1)
            {
                fixed (VkPhysicalDevice* ptr = physicalDevices)
                {
                    vkEnumeratePhysicalDevices(instance, &count, ptr);
                }
            }

            handle = physicalDevicesptr[0];
            handles = device_count is 1 ? new[] { physicalDevicesptr[0] } : physicalDevices;

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
                vkGetPhysicalDeviceFormatProperties(handle, (VkFormat)format, out VkFormatProperties formatProps);

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

            if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.Validation)
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    Log.Error("Vulkan", $"Validation: {messageSeverity} - {message}");

                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
                {
                    Log.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
                }

            }
            else
            {
                if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
                {
                    Log.Error("Vulkan", $"[Vulkan]: {messageSeverity} - {message}");
                }
                else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
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


            vkGetPhysicalDeviceFeatures2(handle, out deviceFeatures2);

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


            vkGetPhysicalDeviceFeatures2(handle, out deviceFeatures2);

            return deviceFeatures2;
        }

        internal void CreatePhysicalDeviceProperties()
        {
            vkGetPhysicalDeviceProperties(handle, out device_properties);
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




        public TDelegate GetInstanceProcAddr<TDelegate>(string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetInstanceProcAddr(instance, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }

        public void Dispose()
        {
            //vkDestroyDebugUtilsMessengerEXT(instance, _debugMessenger, null);
            vkDestroyInstance(instance, null);
        }
    }
}
