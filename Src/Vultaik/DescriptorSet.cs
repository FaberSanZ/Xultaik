// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    public unsafe class DescriptorSet : GraphicsResource
    {

        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;
        internal uint _count = 0;
        internal List<ResourceData> Resources = new();


        public DescriptorSet(GraphicsPipeline pipeline, DescriptorData data) : base(pipeline.NativeDevice)
        {

            VkDescriptorSetLayout descriptor_set_layout = pipeline._descriptorSetLayout;
            NativeDevice._descriptorPoolManager_0.Allocate(descriptor_set_layout);
            _descriptorSet = NativeDevice._descriptorPoolManager_0.handle;

            PipelineState = pipeline;
            DescriptorData = data;

            Build();
        }

        public GraphicsPipeline PipelineState { get; }

        public DescriptorData DescriptorData { get; }

        public void Build()
        {
            Resources = DescriptorData.Data;


            int resources_count = Resources.Count;

            VkWriteDescriptorSet* ptr = stackalloc VkWriteDescriptorSet[resources_count];
            VkDescriptorBufferInfo* bufferInfos = stackalloc VkDescriptorBufferInfo[resources_count];
            VkDescriptorImageInfo* imageInfos = stackalloc VkDescriptorImageInfo[resources_count];


            List<VkWriteDescriptorSet> descriptorSets = new();

            for (int i = 0; i < resources_count; i++)
            {
                ResourceData r = Resources[i];

                if (r.DescriptorType == VkDescriptorType.UniformBuffer)
                {

                    bufferInfos[i] = new()
                    {
                        buffer = r.Buffer.handle,
                        offset = (ulong)r.Offset,
                        range = (ulong)r.Buffer.SizeInBytes,
                    };

                    VkWriteDescriptorSet write_descriptor = new()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        pNext = null,
                        dstSet = _descriptorSet,
                        descriptorCount = 1,
                        descriptorType = r.DescriptorType,
                        pBufferInfo = &bufferInfos[i],
                        dstBinding = (uint)r.Binding,
                    };


                    ptr[i] = write_descriptor;
                }

                else if (r.DescriptorType == VkDescriptorType.CombinedImageSampler)
                {
                    imageInfos[i] = new()
                    {
                        imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                        imageView = r.Texture.image_view,
                        sampler = r.Sampler.handle,
                    };

                    VkWriteDescriptorSet write_descriptor = new()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = (uint)r.Binding,
                        dstArrayElement = 0,
                        descriptorType = r.DescriptorType,
                        descriptorCount = 1,
                        pImageInfo = &imageInfos[i],

                    };


                    ptr[i] = write_descriptor;

                }

                else if (r.DescriptorType == VkDescriptorType.SampledImage)
                {
                    imageInfos[i] = new()
                    {
                        imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                        imageView = r.Texture.image_view,
                    };

                    VkWriteDescriptorSet write_descriptor = new()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = (uint)r.Binding,
                        dstArrayElement = 0,
                        descriptorType = r.DescriptorType,
                        descriptorCount = 1,
                        pImageInfo = &imageInfos[i],

                    };


                    ptr[i] = write_descriptor;

                }

                else if (r.DescriptorType == VkDescriptorType.Sampler)
                {
                    imageInfos[i] = new()
                    {
                        sampler = r.Sampler.handle,
                    };

                    VkWriteDescriptorSet write_descriptor = new()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = (uint)r.Binding,
                        descriptorType = r.DescriptorType,
                        descriptorCount = 1,
                        pImageInfo = &imageInfos[i],

                    };


                    ptr[i] = write_descriptor;

                }
            }

            vkUpdateDescriptorSets(NativeDevice.handle, (uint)resources_count, ptr, 0, null);
        }


        public void Free()
        {
            //vkFreeDescriptorSets()
        }

    }

}
