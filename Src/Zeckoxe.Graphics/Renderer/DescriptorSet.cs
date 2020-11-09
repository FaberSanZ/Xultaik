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
    public class Resource
    {
        public Resource()
        {

        }
        public Resource(GraphicsResource type, ulong offset, int binding = 0)
        {
            ResourceType = type;
            Offset = offset;
            Binding = binding;
        }

        public GraphicsResource ResourceType { get; set; }
        public ulong Offset { get; set; }
        public int Binding { get; set; }

    }



    public class ImageSampler : GraphicsResource
    {
        public ImageSampler(Sampler sampler, Texture text) : base()
        {
            Texture = text;
            Sampler = sampler;
        }



        public Texture Texture { get; set; }
        public Sampler Sampler { get; set; }


    }
    public unsafe class DescriptorSet : GraphicsResource
    {

        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;
        private uint count = 0;

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



        public void SetValues(List<Resource> resources)
        {

            var count = 0;

            VkWriteDescriptorSet* ptr = stackalloc VkWriteDescriptorSet[resources.Count];

            foreach (var r in resources)
            {

                if (r.ResourceType is Buffer buffer)
                {

                    VkDescriptorBufferInfo descriptor = new VkDescriptorBufferInfo
                    {
                        buffer = buffer.handle,
                        offset = r.Offset,
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
                        dstBinding = (uint)r.Binding,
                    };

                    ptr[count++] = writeDescriptorSet;
                }


                if (r.ResourceType is ImageSampler image_sampler)
                {

                    VkDescriptorImageInfo imageInfo;
                    imageInfo.imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
                    imageInfo.imageView = image_sampler.Texture.View;
                    imageInfo.sampler = image_sampler.Sampler.handle;

                    var image_sampler_Writes = new VkWriteDescriptorSet
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = 1,
                        dstArrayElement = 0,
                        descriptorType = VkDescriptorType.CombinedImageSampler,
                        descriptorCount = 1,
                        pImageInfo = &imageInfo,
                    };


                    ptr[count++] = image_sampler_Writes;
                }



                if (r.ResourceType is Sampler sampler)
                {

                    VkDescriptorImageInfo imageInfo = new VkDescriptorImageInfo
                    {
                        sampler = sampler.NativeSampler
                    };

                    VkWriteDescriptorSet sampler_Writes = new VkWriteDescriptorSet
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        descriptorCount = 1,

                        dstSet = _descriptorSet,
                        dstBinding = (uint)r.Offset,
                        dstArrayElement = 0,
                        descriptorType = VkDescriptorType.Sampler,
                        pImageInfo = &imageInfo,
                    };


                    ptr[count++] = sampler_Writes;
                }



                if (r.ResourceType is Texture texture)
                {

                    VkDescriptorImageInfo imageInfo;
                    imageInfo.imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
                    imageInfo.imageView = texture.View;

                    var texture_Writes = new VkWriteDescriptorSet
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = (uint)r.Offset,
                        dstArrayElement = 0,
                        descriptorType = VkDescriptorType.SampledImage,
                        descriptorCount = 1,
                        pImageInfo = &imageInfo,
                    };



                    ptr[count++] = texture_Writes;
                }


            }

            vkUpdateDescriptorSets(NativeDevice.handle, (uint)count, ptr, 0, null);



        }




        public void Build()
        {

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

        }

    }

}
