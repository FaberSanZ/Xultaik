// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsDevice.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Zeckoxe.Graphics;
//using Zeckoxe.Collections;
using Zeckoxe.Core;
using Vulkan;
using static Vulkan.VulkanNative;
using System.Diagnostics;
using System.IO;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsDevice :  IDisposable
    {

        // public
        public GraphicsAdapter NativeAdapter { get; set; }

        public GraphicsInstance NativeInstance { get; set; }

        public GraphicsSwapChain NativeSwapChain { get; set; }

        public PresentationParameters NativeParameters { get; set; }

        public CommandList NativeCommandList { get; set; }

        public uint GraphicsFamily { get; private set; }

        public uint ComputeFamily { get; private set; }

        public uint TransferFamily { get; private set; }



        // internal
        internal VkDevice Device;
        internal VkPhysicalDeviceMemoryProperties MemoryProperties;
        internal List<VkQueueFamilyProperties> QueueFamilyProperties;
        internal VkQueue NativeCommandQueue;
        internal VkPhysicalDeviceProperties Properties;
        internal VkPhysicalDeviceFeatures Features;
        internal VkCommandPool NativeCommandPool;
        internal VkCommandBuffer NativeCommandBufferPrimary;
        internal VkCommandBuffer NativeCommandBufferSecondary;
        internal VkSemaphore ImageAvailableSemaphore;
        internal VkSemaphore RenderFinishedSemaphore;


        public GraphicsDevice(GraphicsAdapter adapter)
        {
            NativeAdapter = adapter;

            NativeInstance = NativeAdapter.DefaultInstance;

            NativeParameters = NativeInstance.Parameters;


            Recreate();
        }


        public void Recreate()
        {
            QueueFamilyProperties = new List<VkQueueFamilyProperties>();


            InitializePlatformDevice();


            NativeSwapChain = new GraphicsSwapChain(this);


            NativeCommandPool = CreateCommandPool();


            NativeCommandBufferPrimary = CreateCommandBufferPrimary();


            NativeCommandList = new CommandList(this);


            NativeCommandBufferSecondary = CreateCommandBufferSecondary();

        }




        public void InitializePlatformDevice()
        {

            // Features should be checked by the examples before using them
            CreateFeatures();



            // Memory properties are used regularly for creating all kinds of buffers
            CreateMemoryProperties();



            // Queue family properties, used for setting up requested queues upon device creation
            CreateQueueFamilyProperties();



            // Get list of supported extensions
            CreateExtensionProperties();



            // Desired queues need to be requested upon logical device creation
            // Due to differing queue family configurations of Vulkan implementations this can be a bit tricky, especially if the application
            // requests different queue types
            CreateDevice();



            // Create CommandQueues
            CreateCommandQueues();


            // Create Semaphores
            ImageAvailableSemaphore = CreateSemaphore();

            RenderFinishedSemaphore = CreateSemaphore();
        }



        internal void CreateFeatures()
        {
            vkGetPhysicalDeviceFeatures(NativeAdapter.NativePhysicalDevice, out VkPhysicalDeviceFeatures features);

            Features = features;
        }



        internal void CreateMemoryProperties()
        {
            VkPhysicalDeviceMemoryProperties memoryProperties;

            vkGetPhysicalDeviceMemoryProperties(NativeAdapter.NativePhysicalDevice, out memoryProperties);

            MemoryProperties = memoryProperties;
        }


        internal void CreateQueueFamilyProperties()
        {
            VkPhysicalDevice physicalDevice = NativeAdapter.NativePhysicalDevice;

            uint Count = 0;

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref Count, null);

            VkQueueFamilyProperties* queueFamilyProperties = stackalloc VkQueueFamilyProperties[(int)Count];

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &Count, queueFamilyProperties);

            for (int i = 0; i < Count; i++)
                QueueFamilyProperties.Add(queueFamilyProperties[i]);
        }


        internal void CreateExtensionProperties()
        {
            uint extCount = 0;

            vkEnumerateDeviceExtensionProperties(NativeAdapter.NativePhysicalDevice, (byte*)null, ref extCount, null);
        }



        internal void CreateDevice()
        {
            VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[3];

            float defaultQueuePriority = 0.0f;

            VkQueueFlags requestedQueueTypes = VkQueueFlags.Graphics | VkQueueFlags.Compute | VkQueueFlags.Transfer;


            // Graphics queue
            if ((requestedQueueTypes & VkQueueFlags.Graphics) != 0)
            {
                GraphicsFamily = GetQueueFamilyIndex(VkQueueFlags.Graphics, QueueFamilyProperties);

                VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = GraphicsFamily,
                    queueCount = 1,
                    pQueuePriorities = &defaultQueuePriority
                };

                queueCreateInfos[0] = (queueInfo);
            }
            else
                GraphicsFamily = (uint)NullHandle;



            // Dedicated compute queue
            if ((requestedQueueTypes & VkQueueFlags.Compute) != 0)
            {
                ComputeFamily = GetQueueFamilyIndex(VkQueueFlags.Compute, QueueFamilyProperties);

                if (ComputeFamily != GraphicsFamily)
                {
                    // If compute family index differs, we need an additional queue create info for the compute queue
                    VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo
                    {
                        sType = VkStructureType.DeviceQueueCreateInfo,
                        queueFamilyIndex = ComputeFamily,
                        queueCount = 1,
                        pQueuePriorities = &defaultQueuePriority
                    };

                    queueCreateInfos[1] = (queueInfo);
                }
            }
            else
                // Else we use the same queue
                ComputeFamily = GraphicsFamily;


            // Dedicated transfer queue
            if ((requestedQueueTypes & VkQueueFlags.Transfer) != 0)
            {
                TransferFamily = GetQueueFamilyIndex(VkQueueFlags.Transfer, QueueFamilyProperties);

                if (TransferFamily != GraphicsFamily && TransferFamily != ComputeFamily)
                {
                    // If compute family index differs, we need an additional queue create info for the transfer queue
                    VkDeviceQueueCreateInfo queueInfo = new VkDeviceQueueCreateInfo
                    {
                        sType = VkStructureType.DeviceQueueCreateInfo,
                        queueFamilyIndex = TransferFamily,
                        queueCount = 1,
                        pQueuePriorities = &defaultQueuePriority
                    };

                    queueCreateInfos[2] = (queueInfo);
                }
            }
            else
                // Else we use the same queue
                TransferFamily = GraphicsFamily;


            // Create the logical device representation
            List<string> deviceExtensions = new List<string>();

            // If the device will be used for presenting to a display via a swapchain we need to request the swapchain extension
            deviceExtensions.Add("VK_KHR_swapchain");

            var oldFeatures = Features;
            VkDeviceCreateInfo deviceCreateInfo = VkDeviceCreateInfo.New();
            deviceCreateInfo.queueCreateInfoCount = 3;
            deviceCreateInfo.pQueueCreateInfos = queueCreateInfos;
            deviceCreateInfo.pEnabledFeatures = &oldFeatures;

            if (deviceExtensions.Count > 0)
            {
                deviceCreateInfo.enabledExtensionCount = (uint)deviceExtensions.Count;
                deviceCreateInfo.ppEnabledExtensionNames = Interop.String.AllocToPointers(deviceExtensions.ToArray());
            }

            vkCreateDevice(NativeAdapter.NativePhysicalDevice, &deviceCreateInfo, null, out var device);

            Device = device;
        }



        internal VkQueue GetQueue(uint queueFamilyIndex = int.MaxValue, uint queueIndex = 0)
        {
            vkGetDeviceQueue(Device, queueFamilyIndex, queueIndex, out var Queue);
            return Queue;
        }


        internal void CreateCommandQueues()
        {
            NativeCommandQueue = GetQueue(GraphicsFamily);
        }





        internal PixelFormat GetSupportedDepthFormat(List<PixelFormat> depthFormats)
        {
            // Since all depth formats may be optional, we need to find a suitable depth format to use
            // Start with the highest precision packed format
            List<PixelFormat> depthFormats2  = new List<PixelFormat>()
            {
                PixelFormat.D32SfloatS8Uint,
                PixelFormat.D32Sfloat,
                PixelFormat.D24UnormS8Uint,
                PixelFormat.D16UnormS8Uint,
                PixelFormat.D16Unorm,
            };

            VkFormatProperties properties = new VkFormatProperties()
            {

            };

            PixelFormat depthFormat = default;

            foreach (PixelFormat format in depthFormats)
            {
                //FormatProperties formatProps = vkGetPhysicalDeviceFormatProperties(physicalDevice,VulkanConvert.ToPixelFormat(format));

                // Format must support depth stencil attachment for optimal tiling
                //if ((formatProps.OptimalTilingFeatures & FormatFeatures.DepthStencilAttachment) != 0)
                    depthFormat = format;

            }

            return depthFormat;
        }


        internal VkShaderModule LoadSPIR_V_Shader(string path)
        {
            byte[] shaderCode = File.ReadAllBytes(path);

            fixed (byte* scPtr = shaderCode)
            {
                // Create a new shader module that will be used for Pipeline creation
                VkShaderModuleCreateInfo moduleCreateInfo = VkShaderModuleCreateInfo.New();
                moduleCreateInfo.codeSize = new UIntPtr((ulong)shaderCode.Length);
                moduleCreateInfo.pCode = (uint*)scPtr;

                vkCreateShaderModule(Device, ref moduleCreateInfo, null, out VkShaderModule shaderModule);

                return shaderModule;
            }
        }



        internal uint GetQueueFamilyIndex(VkQueueFlags queueFlags, List<VkQueueFamilyProperties> queueFamilyProperties)
        {
            // Dedicated queue for compute
            // Try to find a queue family index that supports compute but not graphics
            if ((queueFlags & VkQueueFlags.Compute) != 0)
                for (uint i = 0; i < queueFamilyProperties.Count(); i++)
                    if (((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0) &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Graphics) == 0)
                        return i;




            // Dedicated queue for transfer
            // Try to find a queue family index that supports transfer but not graphics and compute
            if ((queueFlags & VkQueueFlags.Transfer) != 0)
                for (uint i = 0; i < queueFamilyProperties.Count(); i++)
                    if (((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0) &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Graphics) == 0 &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Compute) == 0)
                        return i;




            // For other queue types or if no separate compute queue is present, return the first one to support the requested flags
            for (uint i = 0; i < queueFamilyProperties.Count(); i++)
                if ((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0)
                    return i;



            throw new InvalidOperationException("Could not find a matching queue family index");
        }


        internal VkSemaphore CreateSemaphore()
        {
            VkSemaphoreCreateInfo vkSemaphoreCreate = new VkSemaphoreCreateInfo()
            {
                sType = VkStructureType.SemaphoreCreateInfo,
                pNext = null,
                flags = 0
            };

            vkCreateSemaphore(Device, &vkSemaphoreCreate, null, out var Semaphore);

            return Semaphore;
        }


        internal VkCommandPool CreateCommandPool()
        {
            VkCommandPoolCreateInfo poolInfo = new VkCommandPoolCreateInfo()
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                queueFamilyIndex = GraphicsFamily,
                flags = 0,
                pNext = null,
            };

            vkCreateCommandPool(Device, &poolInfo, null, out var commandPool);

            return commandPool;
        }



        internal VkCommandBuffer CreateCommandBufferPrimary()
        {
            VkCommandBufferAllocateInfo allocInfo = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = NativeCommandPool,

                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };

            vkAllocateCommandBuffers(Device, ref allocInfo, out var commandBuffers);

            return commandBuffers;
        }


        internal VkCommandBuffer CreateCommandBufferSecondary()
        {
            VkCommandBufferAllocateInfo allocInfo = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = NativeCommandPool,

                level = VkCommandBufferLevel.Secondary,
                commandBufferCount = 1,
            };

            vkAllocateCommandBuffers(Device, ref allocInfo, out var commandBuffers);

            return commandBuffers;
        }


        internal VkMemoryType GetMemoryType(VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
        {
            return (&memoryProperties.memoryTypes_0)[index];
        }


        internal uint GetMemoryType(uint typeBits, VkMemoryPropertyFlags properties)
        {

            for (uint i = 0; i < MemoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) == 1)
                    if ((GetMemoryType(MemoryProperties, i).propertyFlags & properties) == properties)
                        return i;


                typeBits >>= 1;
            }


            throw new InvalidOperationException("Could not find a matching memory type");
        }


        public void WaitIdle()
        {
            vkQueueWaitIdle(NativeCommandQueue);
        }



        public void Dispose()
        {

        }
    }

}
