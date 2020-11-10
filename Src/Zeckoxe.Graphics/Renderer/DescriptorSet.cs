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


    public unsafe class DescriptorSet : GraphicsResource
    {

        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;
        internal uint count = 0;


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



        internal List<BufferInfo> buffers = new();
        internal List<ImageSamplerInfo> images = new();
        internal List<ResourceInfo> resourceInfos = new();



        internal class ImageSamplerInfo
        {

            public Texture _texture { get; set; }
            public Sampler _sampler { get; set; }


            internal int _offset { get; set; }
            internal int _binding { get; set; }


            public ImageSamplerInfo(Texture texture, Sampler sampler, int offset, int binding)
            {
                _texture = texture;
                _sampler = sampler;
                _offset = offset;
                _binding = binding;
            }
        }



        internal struct ResourceInfo
        {

            internal Buffer _buffer { get; set; }
            internal Texture _texture { get; set; }
            internal Sampler _sampler { get; set; }


            internal bool is_buffer { get; set; }
            internal bool is_texture { get; set; }
            internal bool is_sampler { get; set; }


            internal int _offset { get; set; }
            internal int _binding { get; set; }

        }


        internal class BufferInfo
        {
            internal Buffer _buffer { get; set; }
            internal int _offset { get; set; }
            internal int _binding { get; set; }


            public BufferInfo(Buffer buffer, int offset, int binding)
            {
                _buffer = buffer;
                _offset = offset;
                _binding = binding;
            }
        }



        public void SetImageSampler(int offset, Texture texture, Sampler sampler)
        {

            resourceInfos.Add(new ResourceInfo
            {
                _offset = offset,
                _binding = offset,
                is_sampler = true,
                is_texture = true,
                _sampler = sampler,
                _texture = texture
            }); ;

            //images.Add(new ImageSamplerInfo(texture, sampler, offset, 0));

        }


        public void SetUniformBuffer(int offset, Buffer buffer, int binding = 0)
        {
            //buffers.Add(new BufferInfo(buffer, offset, binding));


            resourceInfos.Add(new ResourceInfo
            {
                _offset = offset,
                _binding = binding,
                is_buffer = true,
                _buffer = buffer
            });

        }

        public void Build()
        {
            int count = 0;
            int resources_count = resourceInfos.Count;

            VkWriteDescriptorSet* ptr = stackalloc VkWriteDescriptorSet[resources_count];

            List<VkWriteDescriptorSet> descriptorSets = new();


            foreach (ResourceInfo r in resourceInfos)
            {
                if (r.is_buffer)
                {
                    VkDescriptorBufferInfo descriptor = new VkDescriptorBufferInfo
                    {
                        buffer = r._buffer.handle,
                        offset = (ulong)r._offset,
                        range = (ulong)r._buffer.SizeInBytes
                    };

                    VkWriteDescriptorSet writeDescriptorSet = new VkWriteDescriptorSet()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        pNext = null,
                        dstSet = _descriptorSet,
                        descriptorCount = 1,
                        descriptorType = VkDescriptorType.UniformBuffer,
                        pBufferInfo = &descriptor,
                        dstBinding = (uint)r._binding,
                    };

                    descriptorSets.Add(writeDescriptorSet);

                    ptr[count++] = writeDescriptorSet;
                }

                if (r.is_sampler && r.is_texture)
                {
                    VkDescriptorImageInfo imageInfo;
                    imageInfo.imageLayout = VkImageLayout.ShaderReadOnlyOptimal;
                    imageInfo.imageView = r._texture.View;
                    imageInfo.sampler = r._sampler.handle;

                    VkWriteDescriptorSet image_sampler_Writes = new VkWriteDescriptorSet
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        dstSet = _descriptorSet,
                        dstBinding = (uint)r._binding,
                        dstArrayElement = 0,
                        descriptorType = VkDescriptorType.CombinedImageSampler,
                        descriptorCount = 1,
                        pImageInfo = &imageInfo,
                    };

                    descriptorSets.Add(image_sampler_Writes);

                    ptr[count++] = image_sampler_Writes;

                }
            }



            //var arrayptr = descriptorSets.ToArray();
            //    fixed(VkWriteDescriptorSet* ptr = arrayptr)

            vkUpdateDescriptorSets(NativeDevice.handle, (uint)count, ptr, 0, null);
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
