// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DescriptorSet.cs
=============================================================================*/

using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Vulkan
{

    internal class ImageSamplerInfo
    {
        public Image _texture { get; set; }
        public Sampler _sampler { get; set; }


        internal int _offset { get; set; }
        internal int _binding { get; set; }


        public ImageSamplerInfo(Image texture, Sampler sampler, int offset, int binding)
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
        internal Image _texture { get; set; }
        
        
        internal Sampler _sampler { get; set; }
        internal bool is_sampler { get; set; }


        internal int _offset { get; set; }
        internal int _binding { get; set; }



        internal VkDescriptorType descriptor_type { get; set; }
        internal VkShaderStageFlags shader_descriptor_type { get; set; }

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
            NativeDevice._descriptorPoolManager_0.Allocate(descriptor_set_layout);
            _descriptorSet = NativeDevice._descriptorPoolManager_0.handle;
        }



        public void SetImageSampler(int offset, Image texture, Sampler sampler)
        {

            resourceInfos.Add(new ResourceInfo
            {
                _offset = offset,
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
            int resources_count = resourceInfos.Count;

            VkWriteDescriptorSet* ptr = stackalloc VkWriteDescriptorSet[resources_count];
            VkDescriptorBufferInfo* bufferInfos = stackalloc VkDescriptorBufferInfo[resources_count];
            VkDescriptorImageInfo* imageInfos = stackalloc VkDescriptorImageInfo[resources_count];


            List<VkWriteDescriptorSet> descriptorSets = new();

            for (int i = 0; i < resources_count; i++)
            {
                ResourceInfo r = resourceInfos[i];

                if (r.is_buffer)
                {

                    bufferInfos[i] = new()
                    {
                        buffer = r._buffer.handle,
                        offset = (ulong)r._offset,
                        range = (ulong)r._buffer.SizeInBytes,
                    };

                    VkWriteDescriptorSet writeDescriptorSet = new VkWriteDescriptorSet()
                    {
                        sType = VkStructureType.WriteDescriptorSet,
                        pNext = null,
                        dstSet = _descriptorSet,
                        descriptorCount = 1,
                        descriptorType = VkDescriptorType.UniformBuffer,
                        pBufferInfo = &bufferInfos[i],
                        dstBinding = (uint)r._binding,
                    };


                    ptr[i] = writeDescriptorSet;
                }

                else if (r.is_sampler && r.is_texture)
                {
                    imageInfos[i] = new()
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
                        pImageInfo = &imageInfos[i],

                    };


                    ptr[i] = image_sampler_Writes;

                }
            }

            vkUpdateDescriptorSets(NativeDevice.handle, (uint)resources_count, ptr, 0, null);
        }




    }

}
