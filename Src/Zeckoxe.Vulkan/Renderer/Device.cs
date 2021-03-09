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

namespace Zeckoxe.Vulkan
{
    public unsafe class Device : IDisposable
    {
        internal VkDevice handle;
        internal VkPhysicalDeviceMemoryProperties _memoryProperties;
        internal VkQueueFamilyProperties[] queue_family_properties;


        internal VkQueue command_queue;
        internal VkQueue graphics_queue;
        internal VkQueue compute_queue;
        internal VkQueue transfer_queue;

        internal VkCommandPool graphics_cmd_pool;
        internal VkCommandPool compute_cmd_pool;
        internal VkCommandPool transfer_cmd_pool;


        internal VkCommandBuffer command_buffer_primary;
        internal VkCommandBuffer command_buffer_secondary;


        internal VkSemaphore image_available_semaphore;
        internal VkSemaphore render_finished_semaphore;
        internal VkSemaphore graphics_timeline_semaphore;
        internal VkSemaphore compute_timeline_semaphore;
        internal VkSemaphore transfer_timeline_semaphore;


        internal uint timeline_fence_graphics = 0;
        internal uint timeline_fence_compute = 0;
        internal uint timeline_fence_transfer = 0;


        internal DescriptorPool _descriptorPoolManager_0;
        internal DescriptorPool _descriptorPoolManager_1;


        internal VkPhysicalDeviceProperties device_properties;
        internal VkPhysicalDeviceSubgroupProperties subgroup_properties;
        internal VkPhysicalDevice8BitStorageFeatures storage_8bit_features;
        internal VkPhysicalDeviceRayTracingPipelineFeaturesKHR ray_tracing_features;
        internal VkPhysicalDeviceAccelerationStructureFeaturesKHR acceleration_structure_features;
        internal VkPhysicalDevice16BitStorageFeatures storage_16bit_features;
        internal VkPhysicalDeviceShaderFloat16Int8Features float16_int8_features;
        internal VkPhysicalDeviceFeatures enabled_features;
        internal VkPhysicalDeviceExternalMemoryHostPropertiesEXT host_memory_properties;
        internal VkPhysicalDeviceMultiviewFeatures multiview_features;
        internal VkPhysicalDeviceImagelessFramebufferFeatures imageless_features;
        internal VkPhysicalDeviceSubgroupSizeControlFeaturesEXT subgroup_size_control_features;
        internal VkPhysicalDeviceSubgroupSizeControlPropertiesEXT subgroup_size_control_properties;
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

        public Device(Adapter adapter)
        {
            NativeAdapter = adapter;

            NativeParameters = NativeAdapter.Parameters;


            Recreate();
        }


        public Adapter NativeAdapter { get; set; }
        public PresentationParameters NativeParameters { get; set; }
        public CommandBuffer NativeCommand { get; set; }
        public uint GraphicsFamily { get; private set; }
        public uint ComputeFamily { get; private set; }
        public uint TransferFamily { get; private set; }
        public List<string> DeviceExtensionsNames { get; private set; } = new();

        public bool RayTracingSupport { get; private set; }


        public void Recreate()
        {
            queue_family_properties = Array.Empty<VkQueueFamilyProperties>();


            InitializePlatformDevice();


        }




        public void InitializePlatformDevice()
        {


            // Memory properties are used regularly for creating all kinds of buffers
            CreateMemoryProperties();



            // Queue family properties, used for setting up requested queues upon device creation
            CreateQueueFamilyProperties();


            // Desired queues need to be requested upon logical device creation
            // Due to differing queue family configurations of Vulkan implementations this can be a bit tricky, especially if the application
            // requests different queue types
            CreateDevice();



            // Create CommandQueues
            CreateCommandQueues();


            // Create Semaphores
            image_available_semaphore = create_semaphore();
            render_finished_semaphore = create_semaphore();


            graphics_cmd_pool = create_command_pool(GraphicsFamily);
            compute_cmd_pool = create_command_pool(ComputeFamily);
            transfer_cmd_pool = create_command_pool(TransferFamily);


            command_buffer_primary = create_command_buffer_primary(graphics_cmd_pool);
            command_buffer_secondary = CreateCommandBufferSecondary();


            NativeCommand = new(this, CommandBufferType.AsyncGraphics);



            _descriptorPoolManager_0 = new(this);

            //_descriptorPoolManager_1 = _descriptorPoolManager_0;
            //_descriptorPoolManager_1.HeapPool.Reset();
        }




        internal void CreateMemoryProperties()
        {
            vkGetPhysicalDeviceMemoryProperties(NativeAdapter.handle, out _memoryProperties);
        }


        internal void CreateQueueFamilyProperties()
        {
            VkPhysicalDevice physicalDevice = NativeAdapter.handle;

            uint count = 0;

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &count, null);
            VkQueueFamilyProperties* queue_family_properties_ptr = stackalloc VkQueueFamilyProperties[(int)count];

            vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &count, queue_family_properties_ptr);

            queue_family_properties = new VkQueueFamilyProperties[count];

            for (int i = 0; i < count; i++)
            {
                queue_family_properties[i] = queue_family_properties_ptr[i];
            }
        }




        // Multi GPU Helper 
        internal int calculate_shared_device_mask(int gpuCount)
        {
            return (1 << gpuCount) - 1;
        }



        internal void CreateDevice()
        {
            VkDeviceQueueCreateInfo* queue_create_infos = stackalloc VkDeviceQueueCreateInfo[3];

            float defaultQueuePriority = 0.0f;

            VkQueueFlags requestedQueueTypes = VkQueueFlags.Graphics | VkQueueFlags.Compute | VkQueueFlags.Transfer;


            // Graphics queue
            if ((requestedQueueTypes & VkQueueFlags.Graphics) is not 0)
            {
                GraphicsFamily = GetQueueFamilyIndex(VkQueueFlags.Graphics, queue_family_properties);

                VkDeviceQueueCreateInfo queue_info = new()
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = GraphicsFamily,
                    queueCount = 1,
                    pQueuePriorities = &defaultQueuePriority
                };

                queue_create_infos[0] = queue_info;
            }
            else
            {
                GraphicsFamily = uint.MinValue;
            }



            // Dedicated compute queue
            if ((requestedQueueTypes & VkQueueFlags.Compute) is not 0)
            {
                ComputeFamily = GetQueueFamilyIndex(VkQueueFlags.Compute, queue_family_properties);

                if (ComputeFamily != GraphicsFamily)
                {
                    // If compute family index differs, we need an additional queue create info for the compute queue
                    VkDeviceQueueCreateInfo queue_info = new()
                    {
                        sType = VkStructureType.DeviceQueueCreateInfo,
                        queueFamilyIndex = ComputeFamily,
                        queueCount = 1,
                        pQueuePriorities = &defaultQueuePriority
                    };

                    queue_create_infos[1] = queue_info;
                }
            }
            else
            {
                // Else we use the same queue
                ComputeFamily = GraphicsFamily;
            }

            float graphics_queue_prio = 0.5f;
            float compute_queue_prio = 1.0f;
            float transfer_queue_prio = 1.0f;

            float* pri = stackalloc float[3]
            {
                graphics_queue_prio,
                compute_queue_prio,
                transfer_queue_prio
            };

            // Dedicated transfer queue
            if ((requestedQueueTypes & VkQueueFlags.Transfer) is not 0)
            {
                TransferFamily = GetQueueFamilyIndex(VkQueueFlags.Transfer, queue_family_properties);

                if (TransferFamily != GraphicsFamily && TransferFamily != ComputeFamily)
                {
                    // If compute family index differs, we need an additional queue create info for the transfer queue
                    VkDeviceQueueCreateInfo queue_info = new()
                    {
                        sType = VkStructureType.DeviceQueueCreateInfo,
                        queueFamilyIndex = TransferFamily,
                        queueCount = 1,
                        pQueuePriorities = pri + 2
                    };

                    queue_create_infos[2] = queue_info;
                }
            }
            else
            {
                // Else we use the same queue
                TransferFamily = GraphicsFamily;
            }


            VkPhysicalDeviceFeatures2 features = new()
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
            };

            storage_8bit_features = new() { sType = VkStructureType.PhysicalDevice8bitStorageFeatures, };
            acceleration_structure_features = new() { sType = VkStructureType.PhysicalDeviceAccelerationStructureFeaturesKHR, };
            descriptor_indexing_features = new() { sType = VkStructureType.PhysicalDeviceDescriptorIndexingFeatures, };


            bool has_pdf2 = NativeAdapter.SupportsPhysicalDeviceProperties2 || (NativeAdapter.SupportsVulkan11Instance && NativeAdapter.SupportsVulkan11Device);

            OptionalDeviceExtensions OptDeviceExt = NativeAdapter.Parameters.Settings.OptionalDeviceExtensions;

            bool OptRayTracing = (OptDeviceExt & OptionalDeviceExtensions.RayTracing) != 0;
            bool OptMultiview = (OptDeviceExt & OptionalDeviceExtensions.Multiview) != 0;
            bool OptShadingRate = (OptDeviceExt & OptionalDeviceExtensions.ShadingRate) != 0;
            bool OptConsRaster= (OptDeviceExt & OptionalDeviceExtensions.ConservativeRasterization) != 0;

            void** ppNext = &features.pNext;

            if (has_pdf2)
            {
                if (NativeAdapter.device_extensions_names.Contains("VK_KHR_8bit_storage"))
                {
                    DeviceExtensionsNames.Add("VK_KHR_8bit_storage");

                    fixed (VkPhysicalDevice8BitStorageFeatures* feature = &storage_8bit_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }



                if (NativeAdapter.device_extensions_names.Contains("VK_KHR_acceleration_structure") && OptRayTracing)
                {
                    DeviceExtensionsNames.Add("VK_KHR_acceleration_structure");
                    fixed (VkPhysicalDeviceAccelerationStructureFeaturesKHR* feature = &acceleration_structure_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                    RayTracingSupport = true;
                }


                if (NativeAdapter.device_extensions_names.Contains("VK_EXT_descriptor_indexing"))
                {
                    DeviceExtensionsNames.Add("VK_EXT_descriptor_indexing");
                    fixed (VkPhysicalDeviceDescriptorIndexingFeatures* feature = &descriptor_indexing_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }






            }



            //DeviceExtensionsNames.Add("VK_KHR_maintenance1");


            if (NativeAdapter.device_extensions_names.Contains("VK_KHR_swapchain"))
            {
                DeviceExtensionsNames.Add("VK_KHR_swapchain");
            }

            VkDeviceCreateInfo deviceCreateInfo = new()
            {
                sType = VkStructureType.DeviceCreateInfo,
                flags = VkDeviceCreateFlags.None,
                queueCreateInfoCount = 3,
                pQueueCreateInfos = queue_create_infos,
            };


            if (NativeAdapter.SupportsVulkan11Device && NativeAdapter.SupportsVulkan11Instance)
            {
                vkGetPhysicalDeviceFeatures2(NativeAdapter.handle, out features);
            }
            else if (NativeAdapter.SupportsPhysicalDeviceProperties2)
            {
                vkGetPhysicalDeviceFeatures2KHR(NativeAdapter.handle, out features);
            }
            else
            {
                vkGetPhysicalDeviceFeatures(NativeAdapter.handle, out features.features);
            }

            if (NativeAdapter.SupportsPhysicalDeviceProperties2)
            {
                deviceCreateInfo.pNext = &features;
            }
            else
            {
                deviceCreateInfo.pEnabledFeatures = &features.features;
            }



            // Only need GetPhysicalDeviceProperties2 for Vulkan 1.1-only code, so don't bother getting KHR variant.

            VkPhysicalDeviceProperties2 props = new()
            {
                sType = VkStructureType.PhysicalDeviceProperties2,
            };

            conservative_rasterization_properties = new()
            {
                sType = VkStructureType.PhysicalDeviceConservativeRasterizationPropertiesEXT,
            };

            ray_tracing_features = new()
            {
                sType = VkStructureType.PhysicalDeviceRayTracingPipelineFeaturesKHR
            };


            ppNext = &props.pNext;


            if (has_pdf2)
            {
                if (NativeAdapter.device_extensions_names.Contains("VK_KHR_ray_tracing_pipeline"))
                {
                    DeviceExtensionsNames.Add("VK_KHR_ray_tracing_pipeline");
                    fixed (VkPhysicalDeviceRayTracingPipelineFeaturesKHR* feature = &ray_tracing_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }
            }


            if (NativeAdapter.device_extensions_names.Contains("VK_EXT_conservative_rasterization"))
            {
                DeviceExtensionsNames.Add("VK_EXT_conservative_rasterization");
                fixed (VkPhysicalDeviceConservativeRasterizationPropertiesEXT* feature = &conservative_rasterization_properties)
                {
                    *ppNext = feature;
                    ppNext = &feature->pNext;
                }
            }


            if (NativeAdapter.SupportsVulkan11Instance && NativeAdapter.SupportsVulkan11Device)
            {
                vkGetPhysicalDeviceProperties2(NativeAdapter.handle, out props);
            }

            if (DeviceExtensionsNames.Any())
            {
                deviceCreateInfo.enabledExtensionCount = (uint)DeviceExtensionsNames.Count;
                deviceCreateInfo.ppEnabledExtensionNames = Interop.String.AllocToPointers(DeviceExtensionsNames.ToArray());
            }



            vkCreateDevice(NativeAdapter.handle, &deviceCreateInfo, null, out handle).CheckResult();

        }



        internal VkQueue get_queue(uint queueFamilyIndex = int.MaxValue, uint queueIndex = 0)
        {
            vkGetDeviceQueue(handle, queueFamilyIndex, queueIndex, out VkQueue queue);
            return queue;
        }


        internal void CreateCommandQueues()
        {
            command_queue = get_queue(GraphicsFamily);
            graphics_queue = get_queue(GraphicsFamily);
            compute_queue = get_queue(ComputeFamily);
            transfer_queue = get_queue(TransferFamily);
        }



        internal VkShaderModule LoadSpir_V_Shader(Span<byte> bytes)
        {

            fixed (byte* ptr = bytes)
            {
                // Create a new shader module that will be used for Pipeline creation
                VkShaderModuleCreateInfo moduleCreateInfo = new()
                {
                    sType = VkStructureType.ShaderModuleCreateInfo,
                    pNext = null,
                    codeSize = new UIntPtr((ulong)bytes.Length),
                    pCode = (uint*)ptr,
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


        internal uint GetQueueFamilyIndex(VkQueueFlags queueFlags, VkQueueFamilyProperties[] queueFamilyProperties)
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


        internal VkSemaphore create_semaphore()
        {
            VkSemaphoreCreateInfo semaphore_create_info = new()
            {
                sType = VkStructureType.SemaphoreCreateInfo,
                pNext = null,
                flags = VkSemaphoreCreateFlags.None
            };

            vkCreateSemaphore(handle, &semaphore_create_info, null, out VkSemaphore semaphore);

            return semaphore;
        }


        internal VkCommandPool create_command_pool(uint index)
        {
            VkCommandPoolCreateInfo pool_create_info = new()
            {
                sType = VkStructureType.CommandPoolCreateInfo,
                queueFamilyIndex = index,
                flags = VkCommandPoolCreateFlags.None,
                pNext = null,
            };

            vkCreateCommandPool(handle, &pool_create_info, null, out VkCommandPool pool);

            return pool;
        }



        internal VkCommandBuffer create_command_buffer_primary(VkCommandPool pool)
        {
            VkCommandBufferAllocateInfo allocInfo = new()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = pool,

                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };

            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }


        internal VkCommandBuffer CreateCommandBufferSecondary()
        {
            VkCommandBufferAllocateInfo allocInfo = new()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = graphics_cmd_pool,

                level = VkCommandBufferLevel.Secondary,
                commandBufferCount = 1,
            };


            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }


        internal VkMemoryType GetMemoryTypeExt(VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
        {
            return (&memoryProperties.memoryTypes_0)[index];
        }


        internal uint GetMemoryType(uint typeBits, VkMemoryPropertyFlags properties)
        {

            for (uint i = 0; i < _memoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) is 1)
                {
                    if ((GetMemoryTypeExt(_memoryProperties, i).propertyFlags & properties) == properties)
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
            if (formats.Length is 1 && formats.First().format is VkFormat.Undefined)
            {
                return new()
                {
                    format = VkFormat.B8G8R8A8UNorm,
                    colorSpace = VkColorSpaceKHR.SrgbNonLinear
                };
            }

            foreach (VkSurfaceFormatKHR availableFormat in formats)
            {
                if (availableFormat.format is VkFormat.B8G8R8A8UNorm && availableFormat.colorSpace is VkColorSpaceKHR.SrgbNonLinear)
                {
                    return availableFormat;
                }
            }

            return formats.First();
        }

        internal VkPresentModeKHR ChooseSwapPresentMode(VkPresentModeKHR[] presentModes)
        {
            //VkPresentModeKHR bestMode = VkPresentModeKHR.FifoKHR;

            foreach (VkPresentModeKHR availablePresentMode in presentModes)
            {
                if (availablePresentMode is VkPresentModeKHR.Mailbox)
                {
                    return availablePresentMode; // MailboxKHR
                }
                else if (availablePresentMode is VkPresentModeKHR.Immediate)
                {
                    return availablePresentMode; // ImmediateKHR;
                }
            }

            return VkPresentModeKHR.Immediate;
        }



        public void WaitIdle()
        {
            //vkQueueWaitIdle(nativeCommandQueue);
            vkDeviceWaitIdle(handle);
        }


        public TDelegate GetInstanceProcAddr<TDelegate>(string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetInstanceProcAddr(NativeAdapter.instance, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
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
