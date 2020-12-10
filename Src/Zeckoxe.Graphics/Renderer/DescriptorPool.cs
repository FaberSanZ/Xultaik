// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PoolAllocator.cs
=============================================================================*/

using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{





    public unsafe class DescriptorPool : GraphicsResource
    {
        internal object _lock = new object();


        internal VkDescriptorSet Set;
        internal VkDescriptorPool Pool;




        public ulong GetPoolHandle()
        {
            return Pool.Handle;
        }

        public ulong GetDescriptorSetHandle()
        {
            return Set.Handle;
        }

        public DescriptorPool(Device device) : base(device)
        {
        }


        public (VkDescriptorSet, VkDescriptorPool) Allocate(VkDescriptorSetLayout setLayout)
        {
            VkDescriptorPool pool = new HeapPool(NativeDevice).CreateObject();

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

            return (set, pool);
        }

        public void Free()
        {
            lock (_lock)
            {
            }
        }




        internal unsafe void DestroyAll()
        {

        }

    }

}
