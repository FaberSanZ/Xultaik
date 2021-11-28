// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;

namespace Vultaik
{
    public unsafe class ComputePipeline : GraphicsResource
    {
        internal VkPipeline handle;

        internal VkPipelineLayout _pipelineLayout;
        internal VkDescriptorSetLayout _descriptorSetLayout;
        internal List<ShaderResource> Resources { get; set; } = new();


        public ComputePipeline(Device device, ComputePipelineDescription description) : base(device)
        {
            Description = description;
            Resources = description.Shader.Resources;
            SetupDescriptorSetLayout();
            CreatePipelineLayout();
            CreatePipelineCache();
            CreatePipeline();
        }

        private void CreatePipelineCache()
        {
        }

        public ComputePipelineDescription Description { get; set; }
        private void SetupDescriptorSetLayout()
        {

            ShaderResource[] resources = Resources.Where(x => x.resource_type != SPIRVCross.spvc_resource_type.PushConstant).ToArray();



            VkDescriptorSetLayoutBinding* layoutBinding = stackalloc VkDescriptorSetLayoutBinding[resources.Length];

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].stage == ShaderStage.Compute)
                {
                    layoutBinding[i] = new VkDescriptorSetLayoutBinding
                    {
                        binding = resources[i].binding,
                        descriptorCount = 4,
                        descriptorType = resources[i].resource_type.StageTVkDescriptorType(false),
                        stageFlags = VkShaderStageFlags.Compute,
                        pImmutableSamplers = null,
                    };
                }
            }

            VkDescriptorSetLayoutCreateInfo descriptorLayout = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutCreateInfo,
                flags = VkDescriptorSetLayoutCreateFlags.None,
                pNext = null,
                bindingCount = (uint)resources.Length,
                pBindings = layoutBinding
            };

            vkCreateDescriptorSetLayout(NativeDevice.handle, &descriptorLayout, null, out _descriptorSetLayout);


        }

        internal void CreatePipelineLayout()
        {
            VkDescriptorSetLayout* descriptor_set_layout = stackalloc VkDescriptorSetLayout[1]
            {
                _descriptorSetLayout
            };


            ShaderResource[] push_constants = Resources.Where(x => x.resource_type == SPIRVCross.spvc_resource_type.PushConstant).ToArray();

            VkPushConstantRange* push_constant = stackalloc VkPushConstantRange[push_constants.Length];


            for (int i = 0; i < push_constants.Length; i++)
            {
                if (push_constants[i].stage == ShaderStage.Compute)
                {
                    push_constant[i] = new()
                    {
                        offset = push_constants[i].offset,
                        size = push_constants[i].size,
                        stageFlags = VkShaderStageFlags.Compute,
                    };
                }
            }
            // Create the Pipeline layout that is used to generate the rendering pipelines that are based on this descriptor set layout
            // In a more complex scenario you would have different Pipeline layouts for different descriptor set layouts that could be reused
            VkPipelineLayoutCreateInfo layout_create_info = new VkPipelineLayoutCreateInfo
            {
                sType = VkStructureType.PipelineLayoutCreateInfo,
                pNext = null,
                flags = VkPipelineLayoutCreateFlags.None,
            };

            if (push_constants.Any())
            {
                layout_create_info.pPushConstantRanges = push_constant;
                layout_create_info.pushConstantRangeCount = (uint)push_constants.Length;
            }

            if (descriptor_set_layout is not null)
            {
                layout_create_info.pSetLayouts = descriptor_set_layout;
                layout_create_info.setLayoutCount = 1;
            }


            vkCreatePipelineLayout(NativeDevice.handle, &layout_create_info, null, out _pipelineLayout);
        }


        public void CreatePipeline()
        {
            ShaderBytecode shader = Description.Shader;


            if (shader.Stage != ShaderStage.Compute)
                return;

            VkPipelineShaderStageCreateInfo shader_stage_create_info = new()
            {
                sType = VkStructureType.PipelineShaderStageCreateInfo,
                pNext = null,
                stage = VkShaderStageFlags.Compute,
                module = NativeDevice.load_spirv_shader(shader.Data),
                pName = Interop.String.ToPointer(shader.EntryPoint),
            };



            VkComputePipelineCreateInfo info = new()
            {
                sType = VkStructureType.ComputePipelineCreateInfo,
                pNext = null,
                flags = VkPipelineCreateFlags.None,

                stage = shader_stage_create_info,
                layout = _pipelineLayout,

                //basePipelineIndex = -1
                //basePipelineHandle = handle_base
            };

            VkPipeline pipeline = VkPipeline.Null;
            vkCreateComputePipelines(NativeDevice.handle, VkPipelineCache.Null, 1, &info, null, &pipeline);
            handle = pipeline;
        }
    }
}
