// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

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

        internal PipelineState _pipelineState;
        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;



        public DescriptorSet(PipelineState pipelineState) : base(pipelineState.NativeDevice)
        {
            _pipelineState = pipelineState;
            SetupDescriptorPool();
        }

        public void SetupDescriptorPool()
        {
            // We need to tell the API the number of max. requested descriptors per type
            VkDescriptorPoolSize typeCount;
            // This example only uses one descriptor type (uniform buffer) and only requests one descriptor of this type
            typeCount.type = VkDescriptorType.UniformBuffer;
            typeCount.descriptorCount = 1;
            // For additional types you need to add new entries in the type count list
            // E.g. for two combined image samplers :
            // typeCounts[1].type = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
            // typeCounts[1].descriptorCount = 2;

            // Create the global descriptor pool
            // All descriptors used in this example are allocated from this pool
            VkDescriptorPoolCreateInfo descriptorPoolInfo = new VkDescriptorPoolCreateInfo
            {
                sType = VkStructureType.DescriptorPoolCreateInfo,
                poolSizeCount = 1,
                pPoolSizes = &typeCount,
                // Set the max. number of descriptor sets that can be requested from this pool (requesting beyond this limit will result in an error)
                maxSets = 1
            };

            vkCreateDescriptorPool(NativeDevice.handle, &descriptorPoolInfo, null, out VkDescriptorPool descriptorPool);
            _descriptorPool = descriptorPool;
        }



        public void SetUniformBuffer(int binding, Buffer buffer)
        {
            VkDescriptorSetLayout descriptorSetLayout = _pipelineState._descriptorSetLayout;

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
                buffer = buffer.Handle,
                offset = 0,
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

    }

}
