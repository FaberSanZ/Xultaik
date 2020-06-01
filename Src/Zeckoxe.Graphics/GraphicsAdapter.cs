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
        // number of GPUs we're rendering to --- if DG is disabled, this is 1
        private readonly int device_count;





        internal VkPhysicalDevice NativePhysicalDevice { get; private set; }
        internal VkPhysicalDevice[] NativePhysicalDevices { get; private set; }
        internal VkPhysicalDeviceProperties Properties { get; private set; }
        internal VkPhysicalDeviceFeatures2 Features2 { get; private set; }
        internal VkPhysicalDeviceRayTracingFeaturesKHR DeviceRayTracingFeatures { get; private set; }
        
        public GraphicsAdapter(GraphicsInstance Instance)
        {

            DefaultInstance = Instance;

            Recreate();
        }




        public GraphicsInstance DefaultInstance { get; private set; }

        public DeviceType DeviceType => (DeviceType)Properties.deviceType;

        public uint VendorId => Properties.vendorID;

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


                if (VendorId != 0x0 || VendorId > 0)
                {

                }

                description += $" - {VendorNameString(VendorId)}";
                

                return description;
            }
        }



        public bool RayTracingSupport
        {
            get
            {
                if (DefaultInstance.PhysicalDeviceProperties2Support)
                {
                    if (DeviceRayTracingFeatures.rayTracing)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
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


            if (DefaultInstance.PhysicalDeviceProperties2Support)
            {
                Features2 = GetPhysicalDeviceFeatures2();
                DeviceRayTracingFeatures = GetPhysicalDeviceFeaturesRayTracing();
            }
            else
            {
                Features2 = new VkPhysicalDeviceFeatures2();
            }

        }


        public VkPhysicalDeviceRayTracingFeaturesKHR GetPhysicalDeviceFeaturesRayTracing()
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



        public VkPhysicalDeviceFeatures2 GetPhysicalDeviceFeatures2()
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

            if (true)
            {

            }



            return physicalDeviceProperties;
        }


        internal VkPhysicalDevice[] GetPhysicalDevices()
        {
            // Physical Device
            uint Count = 0;
            vkEnumeratePhysicalDevices(DefaultInstance.NativeInstance, &Count, null);

            // Enumerate devices
            VkPhysicalDevice[] physicalDevices = new VkPhysicalDevice[(int)Count];

            fixed (VkPhysicalDevice* ptr = physicalDevices)
            {
                vkEnumeratePhysicalDevices(DefaultInstance.NativeInstance, &Count, ptr);
            }

            return physicalDevices;
        }

    }
}
