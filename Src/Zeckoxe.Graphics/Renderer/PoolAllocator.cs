// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PoolAllocator.cs
=============================================================================*/

using System.Collections.Generic;
using System.Diagnostics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class PoolAllocator : GraphicsResource
    {
        internal List<PoolInfo> _pools = new List<PoolInfo>();
        internal object _lock = new object();

        public PoolAllocator(Device device) : base(device)
        {
            _pools.Add(CreateNewPool());
        }


        public DescriptorAllocationToken Allocate(DescriptorResourceCounts counts, VkDescriptorSetLayout setLayout)
        {
            VkDescriptorPool pool = GetPool(counts);

            VkDescriptorSetAllocateInfo descriptor_set_allocate_info = new()
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                pNext = null,
                descriptorSetCount = 1,
                pSetLayouts = &setLayout,
                descriptorPool = pool,
            };


            VkDescriptorSet set;
            vkAllocateDescriptorSets(NativeDevice.handle, &descriptor_set_allocate_info, &set).CheckResult();

            return new DescriptorAllocationToken(set, pool);
        }

        public void Free(DescriptorAllocationToken token, DescriptorResourceCounts counts)
        {
            lock (_lock)
            {
                foreach (PoolInfo pool in _pools)
                {
                    if (pool.Pool == token.Pool)
                    {
                        pool.Free(NativeDevice.handle, token, counts);
                    }
                }
            }
        }

        internal VkDescriptorPool GetPool(DescriptorResourceCounts counts)
        {
            lock (_lock)
            {
                foreach (PoolInfo pool in _pools)
                {
                    if (pool.Allocate(counts))
                    {
                        return pool.Pool;
                    }
                }

                PoolInfo new_pool = CreateNewPool();

                _pools.Add(new_pool);

                bool result = new_pool.Allocate(counts);

                Debug.Assert(result);

                return new_pool.Pool;
            }
        }

        public unsafe PoolInfo CreateNewPool()
        {
            uint totalSets = 1000;
            uint descriptorCount = 100;
            uint poolSizeCount = 7;
            VkDescriptorPoolSize* sizes = stackalloc VkDescriptorPoolSize[(int)poolSizeCount];

            sizes[0] = new ()
            {
                type = VkDescriptorType.UniformBuffer,
                descriptorCount = descriptorCount
            };

            sizes[1] = new VkDescriptorPoolSize
            {
                type = VkDescriptorType.SampledImage,
                descriptorCount = descriptorCount
            };

            sizes[2] = new()
            {
                type = VkDescriptorType.Sampler,
                descriptorCount = descriptorCount
            };

            sizes[3] = new()
            {
                type = VkDescriptorType.StorageBuffer,
                descriptorCount = descriptorCount
            };

            sizes[4] = new VkDescriptorPoolSize
            {
                type = VkDescriptorType.StorageImage,
                descriptorCount = descriptorCount
            };

            sizes[5] = new()
            {
                type = VkDescriptorType.UniformBufferDynamic,
                descriptorCount = descriptorCount
            };

            sizes[6] = new()
            {
                type = VkDescriptorType.StorageBufferDynamic,
                descriptorCount = descriptorCount
            };


            VkDescriptorPoolCreateInfo pool_create_info  = new()
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                flags = VkDescriptorPoolCreateFlags.FreeDescriptorSet,
                pNext = null,
                maxSets = totalSets,
                pPoolSizes = sizes,
                poolSizeCount = poolSizeCount,
            };

            vkCreateDescriptorPool(NativeDevice.handle, &pool_create_info, null, out VkDescriptorPool descriptorPool).CheckResult();

            return new PoolInfo(descriptorPool, totalSets, descriptorCount);
        }

        internal unsafe void DestroyAll()
        {
            foreach (PoolInfo pool in _pools)
            {
                vkDestroyDescriptorPool(NativeDevice.handle, pool.Pool, null);
            }
        }

    }



    public unsafe class PoolInfo
    {
        public readonly VkDescriptorPool Pool;

        public uint RemainingSets;

        public uint UniformBufferCount;
        public uint SampledImageCount;
        public uint SamplerCount;
        public uint StorageBufferCount;
        public uint StorageImageCount;

        public PoolInfo(VkDescriptorPool pool, uint totalSets, uint descriptorCount)
        {
            Pool = pool;
            RemainingSets = totalSets;
            UniformBufferCount = descriptorCount;
            SampledImageCount = descriptorCount;
            SamplerCount = descriptorCount;
            StorageBufferCount = descriptorCount;
            StorageImageCount = descriptorCount;
        }

        public bool Allocate(DescriptorResourceCounts counts)
        {
            if (RemainingSets > 0
                && UniformBufferCount >= counts.UniformBufferCount
                && SampledImageCount >= counts.SampledImageCount
                && SamplerCount >= counts.SamplerCount
                && StorageBufferCount >= counts.SamplerCount
                && StorageImageCount >= counts.StorageImageCount)
            {
                RemainingSets -= 1;
                UniformBufferCount -= counts.UniformBufferCount;
                SampledImageCount -= counts.SampledImageCount;
                SamplerCount -= counts.SamplerCount;
                StorageBufferCount -= counts.StorageBufferCount;
                StorageImageCount -= counts.StorageImageCount;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Free(VkDevice device, DescriptorAllocationToken token, DescriptorResourceCounts counts)
        {
            VkDescriptorSet set = token.Set;
            vkFreeDescriptorSets(device, Pool, 1, &set);

            RemainingSets += 1;

            UniformBufferCount += counts.UniformBufferCount;
            SampledImageCount += counts.SampledImageCount;
            SamplerCount += counts.SamplerCount;
            StorageBufferCount += counts.StorageBufferCount;
            StorageImageCount += counts.StorageImageCount;
        }
    }


}
