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
        internal bool is_buffer { get; set; }
        internal Buffer _buffer { get; set; }


        internal bool is_texture { get; set; }
        internal Texture _texture { get; set; }
        
        
        internal Sampler _sampler { get; set; }
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


    public unsafe class DescriptorSet : GraphicsResource
    {

        internal VkDescriptorPool _descriptorPool;
        internal VkDescriptorSet _descriptorSet;

        internal uint count = 0;




        internal List<BufferInfo> buffers = new();
        internal List<ImageSamplerInfo> images = new();
        internal List<ResourceInfo> resourceInfos = new();


        public DescriptorSet(GraphicsPipelineState pipelineState) : base(pipelineState.NativeDevice)
        {
            PipelineState = pipelineState;
            SetupDescriptorPool();

        }


        public GraphicsPipelineState PipelineState { get; set; }
        public List<DescriptorPool> Descriptors { get; set; }



        internal void SetupDescriptorPool()
        {

            VkDescriptorSetLayout descriptor_set_layout = PipelineState._descriptorSetLayout;

            _descriptorSet = NativeDevice._descriptorPoolManager_0.Allocate(descriptor_set_layout);
        }


        public void SetTexture2DSampler(int offset, Texture2D texture, Sampler sampler)
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
            buffers.Add(new BufferInfo(buffer, offset, binding));


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
            int resources_count = resourceInfos.Count /*+ buffers.Count - 1*/;

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
                    VkDescriptorImageInfo imageInfo = new()
                    {
                        imageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                        imageView = r._texture.image_view,
                        sampler = r._sampler.handle,
                    };


                    VkWriteDescriptorSet image_sampler_Writes = new()
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
            //fixed(VkWriteDescriptorSet* ptr = arrayptr)

            vkUpdateDescriptorSets(NativeDevice.handle, (uint)count, ptr, 0, null);
        }




    }

}
