// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	GraphicsDevice.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class GraphicsDevice : IDisposable
    {
        internal VkDevice handle;
        internal VkPhysicalDeviceMemoryProperties _memoryProperties;
        internal List<VkQueueFamilyProperties> queueFamilyProperties;
        internal VkQueue nativeCommandQueue;
        internal VkPhysicalDeviceProperties _properties;
        internal VkPhysicalDeviceFeatures _features;
        internal VkCommandPool nativeCommandPool;
        internal VkCommandBuffer nativeCommandBufferPrimary;
        internal VkCommandBuffer nativeCommandBufferSecondary;
        internal VkSemaphore imageAvailableSemaphore;
        internal VkSemaphore renderFinishedSemaphore;


        public GraphicsDevice(GraphicsAdapter adapter)
        {
            NativeAdapter = adapter;

            NativeParameters = NativeAdapter.Parameters;


            Recreate();
        }


        public GraphicsAdapter NativeAdapter { get; set; }
        public PresentationParameters NativeParameters { get; set; }
        public CommandBuffer NativeCommand { get; set; }
        public uint GraphicsFamily { get; private set; }
        public uint ComputeFamily { get; private set; }
        public uint TransferFamily { get; private set; }


        public void Recreate()
        {
            queueFamilyProperties = new List<VkQueueFamilyProperties>();


            InitializePlatformDevice();


            nativeCommandPool = CreateCommandPool();
            

            nativeCommandBufferPrimary = CreateCommandBufferPrimary();


            NativeCommand = new CommandBuffer(this, CommandBufferType.AsyncGraphics | CommandBufferType.AsyncTransfer | CommandBufferType.AsyncCompute); //TODO: CommandBufferType


            nativeCommandBufferSecondary = CreateCommandBufferSecondary();


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
            imageAvailableSemaphore = CreateSemaphore();

            renderFinishedSemaphore = CreateSemaphore();
        }


        internal void CreateFeatures()
        {
            vkGetPhysicalDeviceFeatures(NativeAdapter.handle, out VkPhysicalDeviceFeatures features);

            _features = features;
        }



        internal void CreateMemoryProperties()
        {
            vkGetPhysicalDeviceMemoryProperties(NativeAdapter.handle, out VkPhysicalDeviceMemoryProperties memoryProperties);

            _memoryProperties = memoryProperties;
        }


        internal void CreateQueueFamilyProperties()
        {
            VkPhysicalDevice physicalDevice = NativeAdapter.handle;

            uint Count = 0;

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &Count, null);
            VkQueueFamilyProperties* queueFamilyPropertiesptr = stackalloc VkQueueFamilyProperties[(int)Count];

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &Count, queueFamilyPropertiesptr);

            for (int i = 0; i < Count; i++)
            {
                queueFamilyProperties.Add(queueFamilyPropertiesptr[i]);
            }
        }


        internal void CreateExtensionProperties()
        {
            uint extCount = 0;

            vkEnumerateDeviceExtensionProperties(NativeAdapter.handle, (byte*)null, &extCount, null);
        }



        internal void CreateDevice()
        {
            VkDeviceQueueCreateInfo* queueCreateInfos = stackalloc VkDeviceQueueCreateInfo[3];

            float defaultQueuePriority = 0.0f;

            VkQueueFlags requestedQueueTypes = VkQueueFlags.Graphics | VkQueueFlags.Compute | VkQueueFlags.Transfer;


            // Graphics queue
            if ((requestedQueueTypes & VkQueueFlags.Graphics) != 0)
            {
                GraphicsFamily = GetQueueFamilyIndex(VkQueueFlags.Graphics, queueFamilyProperties);

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
            {
                GraphicsFamily = uint.MinValue;
            }



            // Dedicated compute queue
            if ((requestedQueueTypes & VkQueueFlags.Compute) != 0)
            {
                ComputeFamily = GetQueueFamilyIndex(VkQueueFlags.Compute, queueFamilyProperties);

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
            {
                // Else we use the same queue
                ComputeFamily = GraphicsFamily;
            }


            // Dedicated transfer queue
            if ((requestedQueueTypes & VkQueueFlags.Transfer) != 0)
            {
                TransferFamily = GetQueueFamilyIndex(VkQueueFlags.Transfer, queueFamilyProperties);

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
            {
                // Else we use the same queue
                TransferFamily = GraphicsFamily;
            }


            // Create the logical device representation
            List<string> deviceExtensions = new List<string>
            {

                // If the device will be used for presenting to a display via a swapchain we need to request the swapchain extension
                "VK_KHR_swapchain"
            };

            VkDeviceCreateInfo deviceCreateInfo = new VkDeviceCreateInfo
            {
                sType = VkStructureType.DeviceCreateInfo,
                pNext = null,
                flags = VkDeviceCreateFlags.None,
                queueCreateInfoCount = 3,
                pQueueCreateInfos = queueCreateInfos,
            };


            fixed (VkPhysicalDeviceFeatures* featuresptr = &_features)
            {
                deviceCreateInfo.pEnabledFeatures = featuresptr;
            }


            if (deviceExtensions.Count > 0)
            {
                deviceCreateInfo.enabledExtensionCount = (uint)deviceExtensions.Count;
                deviceCreateInfo.ppEnabledExtensionNames = Interop.String.AllocToPointers(deviceExtensions.ToArray());
            }

            vkCreateDevice(NativeAdapter.handle, &deviceCreateInfo, null, out VkDevice device);

            handle = device;
        }



        internal VkQueue GetQueue(uint queueFamilyIndex = int.MaxValue, uint queueIndex = 0)
        {
            //VkQueue Queue;
            vkGetDeviceQueue(handle, queueFamilyIndex, queueIndex, out VkQueue Queue);
            return Queue;
        }


        internal void CreateCommandQueues()
        {
            nativeCommandQueue = GetQueue(GraphicsFamily);
        }





        internal VkShaderModule LoadSpir_V_Shader(byte[] bytes)
        {

            fixed (byte* scPtr = bytes)
            {
                // Create a new shader module that will be used for Pipeline creation
                VkShaderModuleCreateInfo moduleCreateInfo = new VkShaderModuleCreateInfo()
                {
                    sType = VkStructureType.ShaderModuleCreateInfo,
                    pNext = null,
                    codeSize = new UIntPtr((ulong)bytes.Length),
                    pCode = (uint*)scPtr,
                };

                vkCreateShaderModule(handle, &moduleCreateInfo, null, out VkShaderModule shaderModule);

                return shaderModule;
            }
        }



        internal uint GetMemoryTypeIndex(uint typeBits, VkMemoryPropertyFlags properties)
        {
            // Iterate over all memory types available for the Device used in this example
            for (uint i = 0; i < _memoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) == 1)
                {
                    if ((_memoryProperties.GetMemoryType(i).propertyFlags & properties) == properties)
                    {
                        return i;
                    }
                }
                typeBits >>= 1;
            }

            throw new InvalidOperationException("Could not find a suitable memory type!");
        }


        internal uint GetQueueFamilyIndex(VkQueueFlags queueFlags, List<VkQueueFamilyProperties> queueFamilyProperties)
        {
            // Dedicated queue for compute
            // Try to find a queue family index that supports compute but not graphics
            if ((queueFlags & VkQueueFlags.Compute) != 0)
            {
                for (uint i = 0; i < queueFamilyProperties.Count(); i++)
                {
                    if (((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0) &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Graphics) == 0)
                    {
                        return i;
                    }
                }
            }




            // Dedicated queue for transfer
            // Try to find a queue family index that supports transfer but not graphics and compute
            if ((queueFlags & VkQueueFlags.Transfer) != 0)
            {
                for (uint i = 0; i < queueFamilyProperties.Count(); i++)
                {
                    if (((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0) &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Graphics) == 0 &&
                        (queueFamilyProperties[(int)i].queueFlags & VkQueueFlags.Compute) == 0)
                    {
                        return i;
                    }
                }
            }




            // For other queue types or if no separate compute queue is present, return the first one to support the requested flags
            for (uint i = 0; i < queueFamilyProperties.Count(); i++)
            {
                if ((queueFamilyProperties[(int)i].queueFlags & queueFlags) != 0)
                {
                    return i;
                }
            }

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

            vkCreateSemaphore(handle, &vkSemaphoreCreate, null, out VkSemaphore Semaphore);

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

            vkCreateCommandPool(handle, &poolInfo, null, out VkCommandPool commandPool);

            return commandPool;
        }



        internal VkCommandBuffer CreateCommandBufferPrimary()
        {
            VkCommandBufferAllocateInfo allocInfo = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = nativeCommandPool,

                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };

            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }


        internal VkCommandBuffer CreateCommandBufferSecondary()
        {
            VkCommandBufferAllocateInfo allocInfo = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = nativeCommandPool,

                level = VkCommandBufferLevel.Secondary,
                commandBufferCount = 1,
            };


            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }


        internal VkMemoryType GetMemoryType(VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
        {
            return (&memoryProperties.memoryTypes_0)[index];
        }


        internal uint GetMemoryType(uint typeBits, VkMemoryPropertyFlags properties)
        {

            for (uint i = 0; i < _memoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) == 1)
                {
                    if ((GetMemoryType(_memoryProperties, i).propertyFlags & properties) == properties)
                    {
                        return i;
                    }
                }

                typeBits >>= 1;
            }


            throw new InvalidOperationException("Could not find a matching memory type");
        }



        internal VkSurfaceFormatKHR ChooseSwapSurfaceFormat(VkSurfaceFormatKHR[] formats)
        {
            if (formats.Length == 1 && formats[0].format == VkFormat.Undefined)
            {
                return new VkSurfaceFormatKHR()
                {
                    format = VkFormat.B8G8R8A8UNorm,
                    colorSpace = VkColorSpaceKHR.SrgbNonLinear
                };
            }

            foreach (VkSurfaceFormatKHR availableFormat in formats)
            {
                if (availableFormat.format == VkFormat.B8G8R8A8UNorm && availableFormat.colorSpace == VkColorSpaceKHR.SrgbNonLinear)
                {
                    return availableFormat;
                }
            }

            return formats[0];
        }

        internal VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] presentModes)
        {
            //VkPresentModeKHR bestMode = VkPresentModeKHR.FifoKHR;

            foreach (VkPresentModeKHR availablePresentMode in presentModes)
            {
                if (availablePresentMode == VkPresentModeKHR.Mailbox)
                {
                    return availablePresentMode; // MailboxKHR
                }
                else if (availablePresentMode == VkPresentModeKHR.Immediate)
                {
                    return availablePresentMode; // ImmediateKHR;
                }
            }

            return VkPresentModeKHR.Immediate;
        }

        //public Size ChooseSwapExtent(VkSurfaceCapabilitiesKHR capabilities, uint width, uint height)
        //{
        //    if (capabilities.currentExtent.Width != int.MaxValue)
        //    {
        //        return capabilities.currentExtent;
        //    }

        //    return new Size()
        //    {
        //        Width = (int)Math.Max(capabilities.minImageExtent.Width, Math.Min(capabilities.maxImageExtent.Width, width)),
        //        Height = (int)Math.Max(capabilities.minImageExtent.Height, Math.Min(capabilities.maxImageExtent.Height, height)),
        //    };
        //}


        public string ExtractVersion(uint _value)
        {

            uint major = _value >> 22;
            uint minor = (_value >> 12) & 0x03FF;
            uint patch = _value & 0x0FFF;

            return $"{major}.{minor}.{patch}";
        }


        public void WaitIdle()
        {
            vkQueueWaitIdle(nativeCommandQueue);
        }



        public TDelegate GetDeviceProcAddr<TDelegate>(string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetDeviceProcAddr(handle, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }
        public void Dispose()
        {

        }
    }

}
