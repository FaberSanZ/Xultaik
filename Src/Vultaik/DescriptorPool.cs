// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{

    public unsafe class DescriptorPool : GraphicsResource
    {

        internal VkDescriptorSet handle;
        internal VkDescriptorPool pool;


        public DescriptorPool(Device device) : base(device)
        {
            HeapPool = new(device);
            HeapPool.Create();

            pool = HeapPool.handle;
        }


        public HeapPool HeapPool { get; internal set; }

        public void Allocate(VkDescriptorSetLayout setLayout)
        {
            uint* variable_desc_counts = stackalloc uint[1]
            {
                (uint)DeviceLimits.BindingsBindless
            };


            VkDescriptorSetVariableDescriptorCountAllocateInfo variable_descriptor_count_alloc_info = new() 
            {
                sType = VkStructureType.DescriptorSetVariableDescriptorCountAllocateInfo,
                descriptorSetCount = 2,
                pDescriptorCounts = variable_desc_counts,
            };


            bool descriptor_indexing = NativeDevice.supports_descriptor_indexing();

            VkDescriptorSetAllocateInfo descriptor_set_allocate_info = new()
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                pNext = descriptor_indexing ? &variable_descriptor_count_alloc_info : null,
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
            HeapPool.Dispose();
        }




        internal unsafe void DestroyAll()
        {

        }

    }

}
