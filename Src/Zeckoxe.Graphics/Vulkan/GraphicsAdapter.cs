// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	GraphicsAdapter.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsAdapter
    {
        
        internal int device_count; // number of GPUs we're rendering to --- if DG is disabled, this is 1
        internal VkPhysicalDevice NativePhysicalDevice;
        internal VkPhysicalDevice[] NativePhysicalDevices;
        internal VkPhysicalDeviceProperties Properties;
        internal VkPhysicalDeviceFeatures2 Features2;
        internal VkPhysicalDeviceRayTracingFeaturesKHR DeviceRayTracingFeatures;


        VkPhysicalDeviceSubgroupProperties subgroup_properties;
        VkPhysicalDevice8BitStorageFeatures storage_8bit_features;
        VkPhysicalDevice16BitStorageFeatures storage_16bit_features;
        VkPhysicalDeviceShaderFloat16Int8Features float16_int8_features;
        VkPhysicalDeviceFeatures enabled_features;
        VkPhysicalDeviceExternalMemoryHostPropertiesEXT host_memory_properties;
        VkPhysicalDeviceMultiviewFeatures multiview_features;
        VkPhysicalDeviceImagelessFramebufferFeatures imageless_features;
        VkPhysicalDeviceSubgroupSizeControlFeaturesEXT subgroup_size_control_features;
        VkPhysicalDeviceSubgroupSizeControlPropertiesEXT subgroup_size_control_properties;
        VkPhysicalDeviceComputeShaderDerivativesFeaturesNV compute_shader_derivative_features;
        VkPhysicalDeviceHostQueryResetFeatures host_query_reset_features;
        VkPhysicalDeviceShaderDemoteToHelperInvocationFeaturesEXT demote_to_helper_invocation_features;
        VkPhysicalDeviceScalarBlockLayoutFeatures scalar_block_features;
        VkPhysicalDeviceUniformBufferStandardLayoutFeatures ubo_std430_features;
        VkPhysicalDeviceTimelineSemaphoreFeatures timeline_semaphore_features;
        VkPhysicalDeviceDescriptorIndexingFeatures descriptor_indexing_features;
        VkPhysicalDeviceDescriptorIndexingProperties descriptor_indexing_properties;
        VkPhysicalDeviceConservativeRasterizationPropertiesEXT conservative_rasterization_properties;
        VkPhysicalDevicePerformanceQueryFeaturesKHR performance_query_features;
        VkPhysicalDeviceSamplerYcbcrConversionFeatures sampler_ycbcr_conversion_features;
        VkPhysicalDeviceDriverProperties driver_properties;




        public GraphicsAdapter(GraphicsInstance Instance)
        {
            DefaultInstance = Instance;
            Recreate();
        }


        public bool SupportsPhysicalDeviceProperties2;
        bool supports_external = false;
        bool supports_dedicated = false;
        bool supports_image_format_list = false;
        bool supports_debug_marker = false;
        bool supports_debug_utils = false;
        bool supports_mirror_clamp_to_edge = false;
        bool supports_google_display_timing = false;
        bool supports_nv_device_diagnostic_checkpoints = false;
        bool supports_vulkan_11_instance = false;
        bool supports_vulkan_11_device = false;
        bool supports_external_memory_host = false;
        bool supports_surface_capabilities2 = false;
        bool supports_full_screen_exclusive = false;
        bool supports_update_template = false;
        bool supports_maintenance_1 = false;
        bool supports_maintenance_2 = false;
        bool supports_maintenance_3 = false;
        bool supports_descriptor_indexing = false;
        bool supports_conservative_rasterization = false;
        bool supports_bind_memory2 = false;
        bool supports_get_memory_requirements2 = false;
        bool supports_draw_indirect_count = false;
        bool supports_draw_parameters = false;
        bool supports_driver_properties = false;
        bool supports_calibrated_timestamps = false;


        public GraphicsInstance DefaultInstance { get; private set; }

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



        public void Recreate()
        {
            NativePhysicalDevices = Array.Empty<VkPhysicalDevice>();

            NativePhysicalDevices = GetPhysicalDevices();

            foreach (VkPhysicalDevice item in NativePhysicalDevices)
            {
                NativePhysicalDevice = item;
            }

            Properties = GetProperties();
            Features2 = GetPhysicalDeviceFeatures2();
            SupportsPhysicalDeviceProperties2 = true;
            DeviceRayTracingFeatures = GetPhysicalDeviceFeaturesRayTracing();

            //Features2 = new VkPhysicalDeviceFeatures2();
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


        internal VkPhysicalDevice[] GetPhysicalDevices()
        {
            // Physical Device
            uint Count = 0;
            vkEnumeratePhysicalDevices(DefaultInstance.handle, &Count, null);

            // Enumerate devices
            VkPhysicalDevice[] physicalDevices = new VkPhysicalDevice[(int)Count];

            fixed (VkPhysicalDevice* ptr = physicalDevices)
            {
                vkEnumeratePhysicalDevices(DefaultInstance.handle, &Count, ptr);
            }

            return physicalDevices;
        }

    }
}
