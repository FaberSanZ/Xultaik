// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DescriptorSet.cs
=============================================================================*/

using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{

    public unsafe class DescriptorSet : GraphicsResource
    {

        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;



        public DescriptorSet(GraphicsPipelineState pipelineState, List<DescriptorPool> descriptors, int maxSets = 1) : base(pipelineState.NativeDevice)
        {
            PipelineState = pipelineState;
            Descriptors = descriptors;
            MaxSets = maxSets;
            SetupDescriptorPool();
        }


        public GraphicsPipelineState PipelineState { get; set; }
        public List<DescriptorPool> Descriptors { get; set; }
        public int MaxSets { get; set; }


        public void SetUniformBuffer(int binding, Buffer buffer, ulong offset = 0)
        {
            VkDescriptorSetLayout descriptorSetLayout = PipelineState._descriptorSetLayout;

            // Allocate a new descriptor set from the global descriptor pool
            VkDescriptorSetAllocateInfo allocInfo = new VkDescriptorSetAllocateInfo
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                descriptorPool = _descriptorPool,
                descriptorSetCount = 1,
                pSetLayouts = &descriptorSetLayout,
            };


            VkDescriptorSet descriptorSetptr;
            vkAllocateDescriptorSets(NativeDevice.handle, &allocInfo, &descriptorSetptr);
            _descriptorSet = descriptorSetptr;

            // Update the descriptor set determining the shader binding points
            // For every binding point used in a shader there needs to be one
            // descriptor set matching that binding point
            VkDescriptorBufferInfo descriptor = new VkDescriptorBufferInfo
            {
                buffer = buffer.handle,
                offset = offset,
                range = (ulong)buffer.SizeInBytes
            };

            VkWriteDescriptorSet writeDescriptorSet = new VkWriteDescriptorSet()
            {
                sType = VkStructureType.WriteDescriptorSet,
                pNext = null,
                dstSet = _descriptorSet,
                descriptorCount = 1,
                descriptorType = VkDescriptorType.UniformBuffer,
                pBufferInfo = &descriptor,
                dstBinding = (uint)binding,
            };

            vkUpdateDescriptorSets(NativeDevice.handle, 1, &writeDescriptorSet, 0, null);
        }



        internal void SetupDescriptorPool()
        {
            int descriptors_count = Descriptors.Count;

            VkDescriptorPoolSize* descriptor_pool_size = stackalloc VkDescriptorPoolSize[descriptors_count];

            for (int i = 0; i < descriptors_count; i++)
            {
                descriptor_pool_size[i] = new VkDescriptorPoolSize
                {
                    descriptorCount = (uint)Descriptors[i].Count,
                    type = (VkDescriptorType)Descriptors[i].Type,
                };
            }

            // Create the global descriptor pool
            // All descriptors used in this example are allocated from this pool
            VkDescriptorPoolCreateInfo descriptorPoolInfo = new VkDescriptorPoolCreateInfo
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                pNext = null,
                flags = VkDescriptorPoolCreateFlags.None, // TODO: VkDescriptorPoolCreateFlags
                poolSizeCount = (uint)descriptors_count,
                pPoolSizes = descriptor_pool_size,
                maxSets = (uint)MaxSets   // Set the max. number of descriptor sets that can be requested from this pool (requesting beyond this limit will result in an error)
            };

            vkCreateDescriptorPool(NativeDevice.handle, &descriptorPoolInfo, null, out _descriptorPool);

        }

    }

}
