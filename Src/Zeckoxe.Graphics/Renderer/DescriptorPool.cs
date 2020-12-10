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

        internal VkDescriptorSet DescriptorSet;


        internal VkDescriptorPool handle;


        public DescriptorPool(Device device) : base(device)
        {
            HeapPool = new HeapPool(device);


            handle = HeapPool.Create();
        }


        public HeapPool HeapPool { get; internal set; }

        internal VkDescriptorSet Allocate(VkDescriptorSetLayout setLayout)
        {

            VkDescriptorSetAllocateInfo descriptor_set_allocate_info = new()
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                pNext = null,
                descriptorSetCount = 1,
                pSetLayouts = &setLayout,
                descriptorPool = handle,
            };


            VkDescriptorSet descriptor_set;
            vkAllocateDescriptorSets(NativeDevice.handle, &descriptor_set_allocate_info, &descriptor_set).CheckResult();

            return descriptor_set;
        }

        public void Free()
        {

        }




        internal unsafe void DestroyAll()
        {

        }

    }

}
