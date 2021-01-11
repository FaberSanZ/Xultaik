// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PoolAllocator.cs
=============================================================================*/

using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Vulkan
{

    public unsafe class DescriptorPool : GraphicsResource
    {

        internal VkDescriptorSet handle;


        internal VkDescriptorPool pool;


        public DescriptorPool(Device device) : base(device)
        {
            HeapPool = new HeapPool(device);
            HeapPool.Create();

            pool = HeapPool.handle;
        }


        public HeapPool HeapPool { get; internal set; }

        internal void Allocate(VkDescriptorSetLayout setLayout)
        {

            VkDescriptorSetAllocateInfo descriptor_set_allocate_info = new()
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                pNext = null,
                descriptorSetCount = 1,
                pSetLayouts = &setLayout,
                descriptorPool = pool,
            };


            VkDescriptorSet descriptor_set;
            vkAllocateDescriptorSets(NativeDevice.handle, &descriptor_set_allocate_info, &descriptor_set).CheckResult();
            handle = descriptor_set;
        }

        public void Free()
        {

        }




        internal unsafe void DestroyAll()
        {

        }

    }

}
