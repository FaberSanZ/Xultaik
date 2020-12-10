// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	HeapPool.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class HeapPool : GraphicsResource, IDisposable
    {
        internal static uint MaxSets = 1024;


        internal readonly Dictionary<VkDescriptorType, uint> MaxDescriptorTypeCounts = new()
        {
            [VkDescriptorType.Sampler] = (MaxSets * 2),

            [VkDescriptorType.CombinedImageSampler] = (MaxSets * 3), // TODO: CombinedImageSampler MaxDescriptorTypeCounts

            [VkDescriptorType.SampledImage] = (MaxSets / 2),

            [VkDescriptorType.StorageImage] = (MaxSets / 64),

            [VkDescriptorType.UniformTexelBuffer] = (MaxSets * 2),

            [VkDescriptorType.StorageTexelBuffer] = (MaxSets / 64),

            [VkDescriptorType.UniformBuffer] = (MaxSets / 2),

            [VkDescriptorType.StorageBuffer] = (MaxSets / 64),

            [VkDescriptorType.UniformBufferDynamic] = (MaxSets / 64),

            [VkDescriptorType.StorageBufferDynamic] = (MaxSets / 64),

            [VkDescriptorType.InputAttachment] = (MaxSets / 64),

            [VkDescriptorType.AccelerationStructureKHR] = (MaxSets / 2), // TODO: AccelerationStructureKHR MaxDescriptorTypeCounts
        };






        public HeapPool(Device device) : base(device)
        {
        }

        internal VkDescriptorPool Create()
        {
            // No allocator ready to be used, let's create a new one


            uint PoolSize = (uint)MaxDescriptorTypeCounts.Count;


            VkDescriptorPoolSize* sizes = stackalloc VkDescriptorPoolSize[(int)PoolSize];


            for (int index = 0; index < PoolSize; index++)
            {
                KeyValuePair<VkDescriptorType, uint> item = MaxDescriptorTypeCounts.ElementAt(index);

                sizes[index] = new()
                {
                    descriptorCount = item.Value,
                    type = item.Key
                };

            }


            VkDescriptorPoolCreateInfo descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                poolSizeCount = PoolSize,
                pPoolSizes = sizes,
                maxSets = MaxSets,
            };


            vkCreateDescriptorPool(NativeDevice.handle, &descriptorPoolCreateInfo, null, out VkDescriptorPool descriptorPool);
            return descriptorPool;
        }

        internal void Reset(VkDescriptorPool obj)
        {
            vkResetDescriptorPool(NativeDevice.handle, obj, VkDescriptorPoolResetFlags.None);
        }

        internal void Destroy(VkDescriptorPool obj)
        {
            vkDestroyDescriptorPool(NativeDevice.handle, obj, null);
        }

        public void Dispose()
        {

        }
    }
}
