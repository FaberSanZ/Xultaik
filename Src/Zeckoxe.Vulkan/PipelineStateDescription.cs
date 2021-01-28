// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PipelineStateDescription.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Vulkan
{
    public class PipelineStateDescription
    {
        internal List<ResourceInfo> resourceInfos = new();

        public PipelineStateDescription()
        {

        }


        public Framebuffer Framebuffer { get; set; }

        public InputAssemblyState InputAssemblyState { get; set; }

        public RasterizationState RasterizationState { get; set; } = new RasterizationState();

        public MultisampleState MultisampleState { get; set; } = new MultisampleState();

        public PipelineVertexInput PipelineVertexInput { get; set; }

        public List<ShaderBytecode> Shaders { get; set; } = new List<ShaderBytecode>();

        public List<DescriptorSetLayout> Layouts { get; set; } = new List<DescriptorSetLayout>();

        public List<PushConstantRange> PushConstants { get; set; } = new List<PushConstantRange>();



        //public void Clear()
        //{
        //    resourceInfos.Clear();
        //    resourceInfos = null;
        //    resourceInfos = new();
        //}


        public void SetImageSampler(int offset, ShaderStage stage, Image texture, Sampler sampler)
        {

            resourceInfos.Add(new ResourceInfo
            {
                _offset = offset,
                _binding = offset,
                is_sampler = true,
                is_texture = true,
                _sampler = sampler,
                _texture = texture,
                descriptor_type = Vortice.Vulkan.VkDescriptorType.CombinedImageSampler,
                shader_descriptor_type = stage.StageToVkShaderStageFlags()
            });
        }


        public void SetUniformBuffer(int binding, ShaderStage stage, Buffer buffer, int offset = 0)
        {
            resourceInfos.Add(new ResourceInfo
            {
                _offset = offset,
                _binding = binding,
                is_buffer = true,
                _buffer = buffer,
                descriptor_type = Vortice.Vulkan.VkDescriptorType.UniformBuffer,
                shader_descriptor_type = stage.StageToVkShaderStageFlags()
            });

        }

    }
}
