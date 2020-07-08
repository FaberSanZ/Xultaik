// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

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


        

        public GraphicsAdapter(GraphicsInstance Instance)
        {

            DefaultInstance = Instance;

            Recreate();
        }


        public GraphicsInstance DefaultInstance { get; private set; }

        public DeviceType DeviceType => (DeviceType)Properties.deviceType;

        public uint VendorId => Properties.vendorID;

        public bool RayTracingSupport => DeviceRayTracingFeatures.rayTracing;

        public string DeviceName
        {
            get
            {
                VkPhysicalDeviceProperties deviceProperties = Properties;
                return Interop.String.FromPointer(deviceProperties.deviceName);
            }
        }

        public string Description
        {
            get
            {
                VkPhysicalDeviceProperties deviceProperties = Properties;

                string description = Interop.String.FromPointer(deviceProperties.deviceName);


                // if (VendorId != 0x0)


                description += $" - {VendorNameString(VendorId)}";
                

                return description;
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
