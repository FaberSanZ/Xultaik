// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;

namespace Vultaik
{
    public unsafe class GraphicsPipeline : GraphicsResource, IDisposable
    {

        internal VkPipeline graphicsPipeline;
        internal VkDescriptorSetLayout _descriptorSetLayout;
        internal VkPipelineCache _pipelineCache;
        internal VkPipelineLayout _pipelineLayout;
        internal List<ShaderResource> Resources { get; set; } = new();

        public GraphicsPipeline(GraphicsPipelineDescription description) : base(description.Framebuffer.NativeDevice)
        {
            PipelineStateDescription = description;
            Recreate();
        }

        public GraphicsPipelineDescription PipelineStateDescription { get; set; }


        private void Recreate()
        {
            foreach (ShaderBytecode item in PipelineStateDescription.Shaders)
            {
                Resources.AddRange(item.Resources);
            }

            SetupDescriptorSetLayout();
            CreatePipelineLayout();

            CreatePipelineCache();



            CreateGraphicsPipeline(PipelineStateDescription);

        }




        private void SetupDescriptorSetLayout()
        {
            var indexing_features = NativeDevice.descriptor_indexing_features;
            var indexing_properties = NativeDevice.descriptor_indexing_properties;


            ShaderResource[] resources = Resources.Where(x => x.resource_type != SPIRVCross.spvc_resource_type.PushConstant).ToArray();

            VkDescriptorSetLayoutBinding* layoutBinding = stackalloc VkDescriptorSetLayoutBinding[resources.Length];

            bool is_array = resources.Where(x => x.is_array).Any();
            bool is_bindless = NativeDevice.supports_descriptor_indexing();

            for (int i = 0; i < resources.Length; i++)
            {
                bool is_dynamic = resources[i].is_dynamic;


                layoutBinding[i] = new VkDescriptorSetLayoutBinding
                {
                    binding = resources[i].binding,
                    descriptorCount = 1,
                    descriptorType = resources[i].resource_type.StageTVkDescriptorType(is_dynamic),
                    stageFlags = resources[i].stage.StageToVkShaderStageFlags(),
                    pImmutableSamplers = null,
                };

                if (is_bindless && layoutBinding[i].descriptorType == VkDescriptorType.SampledImage)
                    layoutBinding[i].descriptorCount = NativeDevice.descriptor_indexing_properties.maxDescriptorSetUpdateAfterBindSampledImages;

                //if (is_bindless && layoutBinding[i].descriptorType == VkDescriptorType.UniformBuffer)
                //    layoutBinding[i].descriptorCount = NativeDevice.descriptor_indexing_properties.maxDescriptorSetUpdateAfterBindUniformBuffers;

            }

            VkDescriptorBindingFlags* descriptor_binding_flags = stackalloc VkDescriptorBindingFlags[4]
            {
                VkDescriptorBindingFlags.VariableDescriptorCount,
                VkDescriptorBindingFlags.PartiallyBound,
                VkDescriptorBindingFlags.UpdateAfterBind,
                VkDescriptorBindingFlags.UpdateUnusedWhilePending,
            };

            VkDescriptorSetLayoutBindingFlagsCreateInfo set_layout_binding_flags_info = new()
            {
                sType = VkStructureType.DescriptorSetLayoutBindingCreateInfoEXT,
                bindingCount = 4,
                pBindingFlags = descriptor_binding_flags,
            };


            VkDescriptorSetLayoutCreateInfo descriptor_layout = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutCreateInfo,
                flags = VkDescriptorSetLayoutCreateFlags.None,
                pNext = is_bindless ? &set_layout_binding_flags_info : null,
                bindingCount = (uint)resources.Length,
                pBindings = layoutBinding
            };

            vkCreateDescriptorSetLayout(NativeDevice.handle, &descriptor_layout, null, out _descriptorSetLayout);


        }


        internal void CreatePipelineLayout()
        {
            VkDescriptorSetLayout* descriptor_set_layout = stackalloc VkDescriptorSetLayout[1]
            {
                _descriptorSetLayout
            };


            ShaderResource[] push_constants = Resources.Where(x => x.resource_type ==  SPIRVCross.spvc_resource_type.PushConstant).ToArray();

            VkPushConstantRange* push_constant = stackalloc VkPushConstantRange[push_constants.Length];


            for (int i = 0; i < push_constants.Length; i++)
            {
                push_constant[i] = new()
                {
                    offset = push_constants[i].offset,
                    size = push_constants[i].size,
                    stageFlags = (VkShaderStageFlags)push_constants[i].stage, //TODO: push_constants VkShaderStageFlags
                };
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








        private void CreatePipelineCache()
        {
            //VkPipelineCacheCreateInfo pipelineCacheCreateInfo = new VkPipelineCacheCreateInfo
            //{
            //    sType = VkStructureType.PipelineCacheCreateInfo,
            //};
            //vkCreatePipelineCache(NativeDevice.handle, &pipelineCacheCreateInfo, null, out _pipelineCache);
        }

        public void Dispose()
        {
            vkDestroyPipeline(NativeDevice.handle, graphicsPipeline, null);
            vkDestroyPipelineLayout(NativeDevice.handle, _pipelineLayout, null);
        }

        private void CreateGraphicsPipeline(GraphicsPipelineDescription description)
        {

            List<ShaderBytecode> shaders = description.Shaders;
            VkPipelineShaderStageCreateInfo* shader_stage_infos = stackalloc VkPipelineShaderStageCreateInfo[shaders.Count];

            for (int i = 0; i < shaders.Count; i++)
            {
                shader_stage_infos[i] = new VkPipelineShaderStageCreateInfo
                {
                    sType = VkStructureType.PipelineShaderStageCreateInfo,
                    pNext = null,
                    stage = (VkShaderStageFlags)shaders[i].Stage,
                    module = NativeDevice.load_spirv_shader(shaders[i].Data),
                    pName = Interop.String.ToPointer(shaders[i].EntryPoint),
                };
            }



            List<VertexInputAttribute> vertexAttribute = description.PipelineVertexInput.VertexAttributeDescriptions;
            VkVertexInputAttributeDescription* attributeDescr = stackalloc VkVertexInputAttributeDescription[vertexAttribute.Count];

            for (int i = 0; i < vertexAttribute.Count; i++)
            {
                attributeDescr[i] = new VkVertexInputAttributeDescription
                {
                    binding = (uint)vertexAttribute[i].Binding,
                    format = vertexAttribute[i].Format,
                    location = (uint)vertexAttribute[i].Location,
                    offset = (uint)vertexAttribute[i].Offset,
                };
            }



            List<VertexInputBinding> vertexBinding = description.PipelineVertexInput.VertexBindingDescriptions;
            VkVertexInputBindingDescription* vertexBindingDesc = stackalloc VkVertexInputBindingDescription[vertexBinding.Count];

            for (int i = 0; i < vertexBinding.Count; i++)
            {
                vertexBindingDesc[i] = new VkVertexInputBindingDescription
                {
                    binding = (uint)vertexBinding[i].Binding,
                    inputRate = vertexBinding[i].InputRate.ConvertVertexInputRate(),
                    stride = (uint)vertexBinding[i].Stride
                };
            }




            VkPipelineVertexInputStateCreateInfo vertex_input_state_info = new VkPipelineVertexInputStateCreateInfo()
            {
                sType = VkStructureType.PipelineVertexInputStateCreateInfo,
                pNext = null,
                vertexBindingDescriptionCount = (uint)vertexBinding.Count,
                pVertexBindingDescriptions = vertexBindingDesc,

                vertexAttributeDescriptionCount = (uint)vertexAttribute.Count,
                pVertexAttributeDescriptions = attributeDescr,
            };



            VkPipelineInputAssemblyStateCreateInfo input_ass_state_info = new VkPipelineInputAssemblyStateCreateInfo()
            {
                sType = VkStructureType.PipelineInputAssemblyStateCreateInfo,
                pNext = null,
                flags = VkPipelineInputAssemblyStateCreateFlags.None,
                primitiveRestartEnable = PipelineStateDescription.InputAssemblyState.PrimitiveRestartEnable,
                topology = PipelineStateDescription.InputAssemblyState.PrimitiveType.ConvertPrimitiveType(),
            };



            VkPipelineRasterizationStateCreateInfo rasterizer_state_info = new VkPipelineRasterizationStateCreateInfo()
            {
                sType = VkStructureType.PipelineRasterizationStateCreateInfo,
                pNext = null,
                flags = VkPipelineRasterizationStateCreateFlags.None,
                //depthBiasSlopeFactor = ~0,
                polygonMode = PipelineStateDescription.RasterizationState.FillMode.FillModeToVkPolygonMode(),
                cullMode = PipelineStateDescription.RasterizationState.CullMode.ConvertCullMode(),
                frontFace = PipelineStateDescription.RasterizationState.FrontFace.ConvertFrontFace(),
                lineWidth = PipelineStateDescription.RasterizationState.LineWidth,
                depthBiasEnable = PipelineStateDescription.RasterizationState.DepthBiasEnable,
                depthClampEnable = PipelineStateDescription.RasterizationState.DepthClampEnable,
                rasterizerDiscardEnable = false,
                depthBiasClamp = PipelineStateDescription.RasterizationState.DepthBiasClamp,
                depthBiasConstantFactor = PipelineStateDescription.RasterizationState.DepthBiasConstantFactor,
            };

                



            VkPipelineMultisampleStateCreateInfo multisampleState_info = new VkPipelineMultisampleStateCreateInfo()
            {
                sType = VkStructureType.PipelineMultisampleStateCreateInfo,
                sampleShadingEnable = false,
                rasterizationSamples = VkSampleCountFlags.Count1,
                minSampleShading = 1f,
                pSampleMask = null,
                alphaToCoverageEnable = false,
                alphaToOneEnable = false,
            };


            VkPipelineColorBlendAttachmentState colorBlendAttachementState = new VkPipelineColorBlendAttachmentState
            {
                colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A,
                blendEnable = false
            };


            float* blendConstants = stackalloc float[4]
            {
                0,
                0,
                0,
                0
            };
            VkPipelineColorBlendStateCreateInfo colorBlending = new VkPipelineColorBlendStateCreateInfo()
            {
                sType = VkStructureType.PipelineColorBlendStateCreateInfo,
                logicOpEnable = false,
                logicOp = VkLogicOp.Copy,
                pAttachments = &colorBlendAttachementState,
                attachmentCount = 1,
                //blendConstants = blendConstants

            };


            VkPipelineColorBlendStateCreateInfo colorBlendState = new VkPipelineColorBlendStateCreateInfo()
            {
                sType = VkStructureType.PipelineColorBlendStateCreateInfo,
                pNext = null,
                attachmentCount = 1,
                pAttachments = &colorBlendAttachementState,
            };


            // Depth and stencil state containing depth and stencil compare and test operations
            // We only use depth tests and want depth tests and writes to be enabled and compare with less or equal
            VkPipelineDepthStencilStateCreateInfo depthStencilState = new VkPipelineDepthStencilStateCreateInfo()
            {
                sType = VkStructureType.PipelineDepthStencilStateCreateInfo
            };
            depthStencilState.depthTestEnable = true;
            depthStencilState.depthWriteEnable = true;
            depthStencilState.depthCompareOp = VkCompareOp.LessOrEqual;
            depthStencilState.depthBoundsTestEnable = false;
            depthStencilState.back.failOp = VkStencilOp.Keep;
            depthStencilState.back.passOp = VkStencilOp.Keep;
            depthStencilState.back.compareOp = VkCompareOp.Always;
            depthStencilState.stencilTestEnable = false;
            depthStencilState.front = depthStencilState.back;



            // Enable dynamic states
            // Most states are baked into the Pipeline, but there are still a few dynamic states that can be changed within a command buffer
            // To be able to change these we need do specify which dynamic states will be changed using this Pipeline. Their actual states are set later on in the command buffer.
            // For this example we will set the viewport and scissor using dynamic states
            VkDynamicState* dynamicStateEnables = stackalloc VkDynamicState[2]
            {
                VkDynamicState.Viewport,
                VkDynamicState.Scissor
            };
            VkPipelineDynamicStateCreateInfo dynamicState = new VkPipelineDynamicStateCreateInfo()
            {
                sType = VkStructureType.PipelineDynamicStateCreateInfo,
                pDynamicStates = dynamicStateEnables,
                dynamicStateCount = 2,
            };

            VkPipelineViewportStateCreateInfo vkPipelineViewportStateCreateInfo = new()
            {
                sType = VkStructureType.PipelineViewportStateCreateInfo,
                scissorCount = 1,
                viewportCount = 1,
            };

            VkGraphicsPipelineCreateInfo pipeline_create_info = new VkGraphicsPipelineCreateInfo()
            {
                sType = VkStructureType.GraphicsPipelineCreateInfo,
                pNext = null,
                stageCount = (uint)shaders.Count,
                pStages = shader_stage_infos,
                pVertexInputState = &vertex_input_state_info,
                pInputAssemblyState = &input_ass_state_info,
                pRasterizationState = &rasterizer_state_info,
                pMultisampleState = &multisampleState_info,
                pColorBlendState = &colorBlendState,
                layout = _pipelineLayout,
                renderPass = description.Framebuffer.renderPass,
                //subpass = 0,
                pDepthStencilState = &depthStencilState,
                pDynamicState = &dynamicState,
                //flags = VkPipelineCreateFlags.None
                pViewportState = &vkPipelineViewportStateCreateInfo,
            };



            VkPipeline pipeline;
            vkCreateGraphicsPipelines(NativeDevice.handle, VkPipelineCache.Null, 1, &pipeline_create_info, null, &pipeline);
            graphicsPipeline = pipeline;


            for (int i = 0; i < shaders.Count; i++)
            {
                if (shader_stage_infos[i].module != VkShaderModule.Null)
                {
                    vkDestroyShaderModule(NativeDevice.handle, shader_stage_infos[i].module, null);
                    //shaderStageCreateInfos[i] = *(VkPipelineShaderStageCreateInfo*)null;
                }
            }
            shaders.Clear();
        }

    }
}
