// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;

namespace Vultaik
{

    public unsafe class Device : IDisposable
    {
        internal VkDevice handle;
        internal VkPhysicalDeviceMemoryProperties _memoryProperties;
        internal VkQueueFamilyProperties[] queue_family_properties;


        internal VkQueue graphics_queue;
        internal VkQueue compute_queue;
        internal VkQueue transfer_queue;

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


        internal const VkBufferUsageFlags UnknownBufferUsage = unchecked((VkBufferUsageFlags)uint.MaxValue);

        public Device(Adapter adapter)
        {
            NativeAdapter = adapter;

            AdapterConfig = NativeAdapter.AdapterConfig;


            Recreate();
        }
        public uint VulkanNumBindingsBindless = 1024;
        public uint VulkanNumBindingsBindlessVarying = 16 * 1024;

        public long SmallHeapMaxSize = 1024L * 1024 * 1024;

        public long MinFreeSuballocationSizeToRegister = 16;
        public int FrameIndexLost = -1;
        public uint CorruptionDetectionMagicValue = 0x7F84E666;

        public byte AllocationFillPattern_Created = 0xDC;
        public byte AllocationFillPattern_Destroyed = 0xEF;
        public bool DebugInitializeAllocations = false;

        public long DebugMargin = 0;
        public long DebugAlignment = 1;
        public long DebugMinBufferImageGranularity = 1;



        public Adapter NativeAdapter { get; set; }
        public AdapterConfig AdapterConfig { get; set; }
        public CommandBuffer GraphicsCommandBuffer { get; set; }
        public uint GraphicsFamily { get; private set; }
        public uint ComputeFamily { get; private set; }
        public uint TransferFamily { get; private set; }
        public List<string> DeviceExtensionsNames { get; private set; } = new();
        public bool RayTracingSupport { get; private set; }

        public bool SamplerAnisotropySupport { get; set; }

        public void Recreate()
        {
            queue_family_properties = Array.Empty<VkQueueFamilyProperties>();


            InitializePlatformDevice();

            // Create Semaphores
            image_available_semaphore = create_semaphore();
            render_finished_semaphore = create_semaphore();


            transfer_cmd_pool = create_command_pool(TransferFamily);

            GraphicsCommandBuffer = new(this, CommandBufferType.AsyncGraphics);

            _descriptorPoolManager_0 = new(this);
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




            VkQueueFlags requested_queue_types = VkQueueFlags.Graphics | VkQueueFlags.Compute | VkQueueFlags.Transfer;
            VkDeviceQueueCreateInfo* queue_create_infos = stackalloc VkDeviceQueueCreateInfo[3];
            float default_queue_priority = 1.0f;
            float graphics_queue_prio = 0.5f;
            float transfer_queue_prio = 1.0f;
            float compute_queue_prio = 1.0f;
            uint queue_count = 0;

            // Graphics queue
            if ((requested_queue_types & VkQueueFlags.Graphics) is not 0)
            {
                GraphicsFamily = GetQueueFamilyIndex(VkQueueFlags.Graphics, queue_family_properties);

                VkDeviceQueueCreateInfo queue_info = new()
                {
                    sType = VkStructureType.DeviceQueueCreateInfo,
                    queueFamilyIndex = GraphicsFamily,
                    queueCount = 1,
                    pQueuePriorities = &default_queue_priority
                };

                queue_create_infos[0] = queue_info;
                queue_count++;
            }
            else
            {
                GraphicsFamily = uint.MinValue;
            }



            // Dedicated compute queue
            if ((requested_queue_types & VkQueueFlags.Compute) is not 0)
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
                        pQueuePriorities = &default_queue_priority
                    };

                    queue_create_infos[1] = queue_info;
                    queue_count++;
                }
            }
            else
            {
                // Else we use the same queue
                ComputeFamily = GraphicsFamily;
            }


            // Dedicated transfer queue
            if ((requested_queue_types & VkQueueFlags.Transfer) is not 0)
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
                        pQueuePriorities = &default_queue_priority
                    };

                    queue_create_infos[2] = queue_info;
                    queue_count++;
                }
            }
            else
            {
                // Else we use the same queue
                TransferFamily = GraphicsFamily;
            }

            VkDeviceCreateInfo device_create_info = new()
            {
                sType = VkStructureType.DeviceCreateInfo,
                flags = VkDeviceCreateFlags.None,
                queueCreateInfoCount = queue_count,
                pQueueCreateInfos = queue_create_infos,
            };

            bool has_vulkan_1_1 = NativeAdapter.SupportsVulkan11Instance && NativeAdapter.SupportsVulkan11Device;
            bool has_physical_device_features2 = NativeAdapter.SupportsPhysicalDeviceProperties2 || has_vulkan_1_1;
            bool has_maintenance1 = NativeAdapter.Maintenance1.Support;
            bool has_maintenance2 = NativeAdapter.Maintenance2.Support;
            bool has_maintenance3 = NativeAdapter.Maintenance3.Support;
            bool has_swapChain = NativeAdapter.SwapChain.implement;
            bool has_bindless = NativeAdapter.Bindless.implement && has_maintenance3;
            bool has_conservative_raster = NativeAdapter.ConservativeRasterization.implement;
            bool has_storage_buffer_storage_class = !NativeAdapter.StorageBufferStorageclass.Support; //TODO: StorageBufferStorageclass
            bool has_16_bit_storage = NativeAdapter.Arithmetic16BitStorage.implement && has_storage_buffer_storage_class;

            if (has_maintenance1)
                DeviceExtensionsNames.Add(NativeAdapter.Maintenance1.Name);

            if (has_maintenance2)
                DeviceExtensionsNames.Add(NativeAdapter.Maintenance2.Name);

            if (has_maintenance3)
                DeviceExtensionsNames.Add(NativeAdapter.Maintenance3.Name);

            if (has_storage_buffer_storage_class)
                DeviceExtensionsNames.Add(NativeAdapter.StorageBufferStorageclass.Name);

            if (has_swapChain)
                DeviceExtensionsNames.Add(NativeAdapter.SwapChain.Name);

            if (has_bindless)
                DeviceExtensionsNames.Add(NativeAdapter.Bindless.Name);

            if (has_conservative_raster)
                DeviceExtensionsNames.Add(NativeAdapter.ConservativeRasterization.Name);

            if (has_16_bit_storage)
                DeviceExtensionsNames.Add(NativeAdapter.Arithmetic16BitStorage.Name);


            VkPhysicalDeviceFeatures2 features = new()
            {
                sType = VkStructureType.PhysicalDeviceFeatures2,
            };

            storage_8bit_features = new() 
            { 
                sType = VkStructureType.PhysicalDevice8bitStorageFeatures, 
            };
            acceleration_structure_features = new() 
            {
                sType = VkStructureType.PhysicalDeviceAccelerationStructureFeaturesKHR, 
            };
            descriptor_indexing_features = new() 
            { 
                sType = VkStructureType.PhysicalDeviceDescriptorIndexingFeatures, 
            };
            ubo_std430_features = new() 
            { 
                sType = VkStructureType.PhysicalDeviceUniformBufferStandardLayoutFeatures, 
            };

            ray_tracing_features = new()
            {
                sType = VkStructureType.PhysicalDeviceRayTracingPipelineFeaturesKHR
            };


            void** ppNext = &features.pNext;
     

            if (has_physical_device_features2)
            {
                if (has_bindless)
                {
                    fixed (VkPhysicalDeviceDescriptorIndexingFeatures* feature = &descriptor_indexing_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }


                if (has_16_bit_storage)
                {
                    fixed (VkPhysicalDevice16BitStorageFeatures* feature = &storage_16bit_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }


                if (NativeAdapter.device_extensions_names.Contains("VK_KHR_acceleration_structure") && AdapterConfig.RayTracing)
                {
                    DeviceExtensionsNames.Add("VK_KHR_acceleration_structure");
                    fixed (VkPhysicalDeviceAccelerationStructureFeaturesKHR* feature = &acceleration_structure_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                    RayTracingSupport = true;
                }


                if (NativeAdapter.device_extensions_names.Contains("VK_KHR_ray_tracing_pipeline") && AdapterConfig.RayTracing)
                {
                    DeviceExtensionsNames.Add("VK_KHR_ray_tracing_pipeline");
                    fixed (VkPhysicalDeviceRayTracingPipelineFeaturesKHR* feature = &ray_tracing_features)
                    {
                        *ppNext = feature;
                        ppNext = &feature->pNext;
                    }
                }





                //if (NativeAdapter.device_extensions_names.Contains("VK_KHR_uniform_buffer_standard_layout"))
                //{
                //    DeviceExtensionsNames.Add("VK_KHR_uniform_buffer_standard_layout");

                //    fixed (VkPhysicalDeviceUniformBufferStandardLayoutFeatures* feature = &ubo_std430_features)
                //    {
                //        feature->uniformBufferStandardLayout = true;
                //        *ppNext = feature;
                //        ppNext = &feature->pNext;
                //    }

                //}

            }


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
                device_create_info.pNext = &features;
            }
            else
            {
                device_create_info.pEnabledFeatures = &features.features;
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

            descriptor_indexing_properties = new()
            {
                sType = VkStructureType.PhysicalDeviceDescriptorIndexingProperties
            };

            ppNext = &props.pNext;

            if (has_bindless)
            {
                fixed (VkPhysicalDeviceDescriptorIndexingProperties* prop = &descriptor_indexing_properties)
                {
                    *ppNext = prop;
                    ppNext = &prop->pNext;
                }
            }


            if (has_conservative_raster)
            {
                fixed (VkPhysicalDeviceConservativeRasterizationPropertiesEXT* prop = &conservative_rasterization_properties)
                {
                    *ppNext = prop;
                    ppNext = &prop->pNext;
                }
            }


            if (NativeAdapter.SupportsVulkan11Instance && NativeAdapter.SupportsVulkan11Device)
            {
                vkGetPhysicalDeviceProperties2(NativeAdapter.handle, out props);
            }

            if (DeviceExtensionsNames.Any())
            {
                device_create_info.enabledExtensionCount = (uint)DeviceExtensionsNames.Count;
                device_create_info.ppEnabledExtensionNames = Interop.String.AllocToPointers(DeviceExtensionsNames.ToArray());
            }


            vkCreateDevice(NativeAdapter.handle, &device_create_info, null, out handle).CheckResult();

        }



        internal VkQueue get_queue(uint queueFamilyIndex = int.MaxValue, uint queueIndex = 0)
        {
            vkGetDeviceQueue(handle, queueFamilyIndex, queueIndex, out VkQueue queue);
            return queue;
        }


        internal void CreateCommandQueues()
        {
            graphics_queue = get_queue(GraphicsFamily);
            compute_queue = get_queue(ComputeFamily);
            transfer_queue = get_queue(TransferFamily);
        }



        internal VkShaderModule load_spirv_shader(Span<byte> bytes)
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


        internal uint get_memory_type(uint typeBits, VkMemoryPropertyFlags properties)
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

        internal QueueData get_queue_data(VkQueueFlags flags, VkQueueFamilyProperties[] queue_family_properties)
        {
            QueueData data = new();

            // Dedicated queue for compute
            // Try to find a queue family index that supports compute but not graphics
            if ((flags & VkQueueFlags.Compute) != 0)
            {
                for (uint i = 0; i < queue_family_properties.Count(); i++)
                {
                    VkQueueFamilyProperties queue = queue_family_properties[(int)i];

                    if (((queue.queueFlags & flags) != 0) && (queue.queueFlags & VkQueueFlags.Graphics) == 0)
                    {
                        data.index = queue.queueCount;
                        data.family_index = i;
                        data.timestamp_valid_bits = queue.timestampValidBits;
                        data.type = queue.queueFlags;

                        return data;
                    }
                }
            }




            // Dedicated queue for transfer
            // Try to find a queue family index that supports transfer but not graphics and compute
            if ((flags & VkQueueFlags.Transfer) != 0)
            {
                for (uint i = 0; i < queue_family_properties.Count(); i++)
                {
                    if (((queue_family_properties[(int)i].queueFlags & flags) != 0) &&
                        (queue_family_properties[(int)i].queueFlags & VkQueueFlags.Graphics) == 0 &&
                        (queue_family_properties[(int)i].queueFlags & VkQueueFlags.Compute) == 0)
                    {
                        return data;
                    }
                }
            }




            // For other queue types or if no separate compute queue is present, return the first one to support the requested flags
            for (uint i = 0; i < queue_family_properties.Count(); i++)
            {
                if ((queue_family_properties[(int)i].queueFlags & flags) != 0)
                {
                    return data;
                }
            }

            throw new InvalidOperationException("Could not find a matching queue family index");
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
                flags = VkCommandPoolCreateFlags.ResetCommandBuffer,
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
                pNext = null,
                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };

            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }


        internal VkCommandBuffer CreateCommandBufferSecondary(VkCommandPool pool)
        {
            VkCommandBufferAllocateInfo allocInfo = new()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = pool,

                level = VkCommandBufferLevel.Secondary,
                commandBufferCount = 1,
            };


            VkCommandBuffer commandBuffers;
            vkAllocateCommandBuffers(handle, &allocInfo, &commandBuffers);

            return commandBuffers;
        }

        public VkQueue get_queue_type_cmd(CommandBuffer cmd)
        {
            if (cmd.Type == CommandBufferType.AsyncGraphics)
                return graphics_queue;

            if (cmd.Type == CommandBufferType.AsyncCompute)
                return compute_queue;

            if (cmd.Type == CommandBufferType.AsyncTransfer)
                return transfer_queue;

            return VkQueue.Null;
        }

        //public void SubmitTransfer(CommandBuffer commandBuffer, Fence? fence = null)
        //{
        //    VkQueue queue = get_queue_type_cmd(commandBuffer);
        //    VkCommandBuffer cmd = commandBuffer.handle;


        //    VkSubmitInfo submit_info = new()
        //    {
        //        sType = VkStructureType.SubmitInfo,
        //        commandBufferCount = 1,
        //        pCommandBuffers = &cmd,
        //    };

        //    vkQueueSubmit(queue, 1, &submit_info, VkFence.Null);
        //}


        //public void Submit(CommandBuffer commandBuffer, Fence? fence = null)
        //{
        //    VkQueue queue = get_queue_type_cmd(commandBuffer);
        //    VkCommandBuffer cmd = commandBuffer.handle;
        //    VkSemaphore signal_semaphore = render_finished_semaphore;
        //    VkSemaphore wait_semaphore = image_available_semaphore;


        //    VkSubmitInfo submit_info = new()
        //    {
        //        sType = VkStructureType.SubmitInfo,
        //        commandBufferCount = 1,
        //        pCommandBuffers = &cmd,
        //    };


        //    submit_info.waitSemaphoreCount = 1;
        //    submit_info.pWaitSemaphores = &wait_semaphore;

        //    submit_info.signalSemaphoreCount = 1;
        //    submit_info.pSignalSemaphores = &signal_semaphore;

        //    vkQueueSubmit(queue, 1, &submit_info, VkFence.Null);
        //}



        public void Submit(CommandBuffer commandBuffer, Fence? fence = null)
        {
            VkPipelineStageFlags wait_stages = VkPipelineStageFlags.ColorAttachmentOutput;
            VkSemaphore signal_semaphore = render_finished_semaphore;
            VkSemaphore wait_semaphore = image_available_semaphore;
            VkQueue queue = get_queue_type_cmd(commandBuffer);
            CommandBufferType cmd_type = commandBuffer.Type;
            VkCommandBuffer cmd = commandBuffer.handle;
            var force_exclusive_transfer_queue = false;
            VkFence sync_fence = VkFence.Null;
            bool use_semaphore = true;

            if (queue == transfer_queue && cmd_type == CommandBufferType.AsyncTransfer)
            {
                wait_stages &= ~VkPipelineStageFlags.ColorAttachmentOutput;

                if (force_exclusive_transfer_queue)
                    wait_stages |= VkPipelineStageFlags.Transfer;

                use_semaphore = false;
            }


            if (queue == graphics_queue && cmd_type == CommandBufferType.AsyncTransfer)
            {
                wait_stages &= ~VkPipelineStageFlags.ColorAttachmentOutput;

                if (force_exclusive_transfer_queue)
                    wait_stages |= VkPipelineStageFlags.Transfer;

                use_semaphore = false;
            }


            if (queue == graphics_queue && cmd_type == CommandBufferType.AsyncCompute)
            {
                wait_stages &= ~VkPipelineStageFlags.ColorAttachmentOutput;
                wait_stages |= VkPipelineStageFlags.ComputeShader;

                use_semaphore = false;
            }

            if (queue == compute_queue && cmd_type == CommandBufferType.AsyncCompute)
            {
                wait_stages &= ~VkPipelineStageFlags.ColorAttachmentOutput;
                wait_stages |= VkPipelineStageFlags.ComputeShader;

                use_semaphore = false;
            }

            VkSubmitInfo submit_info = new()
            {
                sType = VkStructureType.SubmitInfo,
                pWaitDstStageMask = &wait_stages,
                pNext = null,
                commandBufferCount = 1,
                pCommandBuffers = &cmd,
            };

            if (use_semaphore)
            {
                submit_info.waitSemaphoreCount = 1;
                submit_info.pWaitSemaphores = &wait_semaphore;

                submit_info.signalSemaphoreCount = 1;
                submit_info.pSignalSemaphores = &signal_semaphore;
            }


            if (fence is not null)
                sync_fence = fence.handle;
            else
                sync_fence = commandBuffer.WaitFence.handle;

            if (sync_fence != VkFence.Null && queue != VkQueue.Null)
                vkQueueSubmit(queue, 1, &submit_info, sync_fence);
        }

        internal VkMemoryType memory_prop_ext(VkPhysicalDeviceMemoryProperties memory, uint index)
        {
            VkMemoryType* ptr = &memory.memoryTypes_0;

            if (ptr[index].propertyFlags == VkMemoryPropertyFlags.DeviceCoherentAMD)
            { }

            return ptr[index];
        }


        internal uint select_memory_type(uint typeBits, VkMemoryPropertyFlags properties)
        {

            for (uint i = 0; i < _memoryProperties.memoryTypeCount; i++)
            {
                if ((typeBits & 1) is 1)
                {
                    if ((memory_prop_ext(_memoryProperties, i).propertyFlags & properties) == properties)
                    {
                        return i;
                    }
                }

                typeBits >>= 1;
            }


            throw new InvalidOperationException("Could not find a matching memory type");
        }



        internal VkSurfaceFormatKHR choose_swap_surface_format(VkSurfaceFormatKHR[] formats)
        {
            if (formats.Length is 1 && formats.First().format is VkFormat.Undefined)
            {
                return new()
                {
                    format = VkFormat.B8G8R8A8UNorm,
                    colorSpace = VkColorSpaceKHR.SrgbNonLinear
                };
            }

            foreach (VkSurfaceFormatKHR format in formats)
                if (format.format is VkFormat.B8G8R8A8UNorm && format.colorSpace is VkColorSpaceKHR.SrgbNonLinear)
                    return format;

            return formats.First();
        }

        internal VkPresentModeKHR choose_swap_present_mode(VkPresentModeKHR[] modes)
        {
            VkPresentModeKHR fifo_mode = VkPresentModeKHR.Fifo;

            foreach (VkPresentModeKHR mode in modes)
            {
                if (mode is VkPresentModeKHR.Mailbox)
                    return mode;

                else if (mode is VkPresentModeKHR.Immediate)
                    return mode; 
            }

            return VkPresentModeKHR.Immediate;
        }



        public void QueueWaitIdleIdle(VkQueue queue)
        {
            if(queue != VkQueue.Null)
                vkQueueWaitIdle(queue).CheckResult();
        }

        internal bool supports_descriptor_indexing()
        {

            bool supports_descriptor_indexing = descriptor_indexing_features.descriptorBindingSampledImageUpdateAfterBind &&
                                                descriptor_indexing_features.descriptorBindingPartiallyBound &&
                                                descriptor_indexing_features.runtimeDescriptorArray &&
                                                descriptor_indexing_features.shaderSampledImageArrayNonUniformIndexing;

            return supports_descriptor_indexing;
        }

        public void WaitIdle()
        {
            if (handle != VkDevice.Null)
                vkDeviceWaitIdle(handle);
        }


        public void ComputeQueueWaitIdle()
        {
            vkQueueWaitIdle(compute_queue);
        }

        public void TransferQueueWaitIdle()
        {
            vkQueueWaitIdle(transfer_queue);
        }



        public TDelegate? GetInstanceProcAddr<TDelegate>(string name) where TDelegate : class
        {
            delegate* unmanaged[Stdcall]<void> funcPtr = vkGetInstanceProcAddr(NativeAdapter.instance, Interop.String.ToPointer(name));

            return null;
        }


        public TDelegate? GetDeviceProcAddr<TDelegate>(string name) where TDelegate : class
        {
            IntPtr funcPtr = vkGetDeviceProcAddr(handle, Interop.String.ToPointer(name));

            return funcPtr != IntPtr.Zero ? Interop.GetDelegateForFunctionPointer<TDelegate>(funcPtr) : null;
        }
        public void Dispose()
        {
            _descriptorPoolManager_0.DestroyAll();
            vkDestroySemaphore(handle, image_available_semaphore, null);
            vkDestroySemaphore(handle, render_finished_semaphore, null);
            vkDestroyCommandPool(handle, transfer_cmd_pool, null);
            _descriptorPoolManager_0.Free();
            vkDeviceWaitIdle(handle);
            vkDestroyDevice(handle, null);
        }
    }

    internal class QueueData
    {
        internal VkQueueFlags type;
        internal uint family_index;
        internal uint index;
        internal uint timestamp_valid_bits = 0;
    }
}
