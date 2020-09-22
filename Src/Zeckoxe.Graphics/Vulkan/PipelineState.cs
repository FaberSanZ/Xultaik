// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineState.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class PipelineState : GraphicsResource
    {

        internal VkPipeline graphicsPipeline;
        internal VkDescriptorSetLayout _descriptorSetLayout;
        internal VkPipelineCache _pipelineCache;
        internal VkDescriptorPool _descriptorPool;
        private VkDescriptorSet _descriptorSet;
        private VkPipelineLayout _pipelineLayout;

        public PipelineState(PipelineStateDescription description) : base(description.Framebuffer.NativeDevice)
        {
            PipelineStateDescription = description;
            Recreate();
        }

        public PipelineStateDescription PipelineStateDescription { get; set; }



        private void Recreate()
        {
            SetupDescriptorSetLayout();
            CreatePipelineCache();

            CreateGraphicsPipeline(PipelineStateDescription);
            SetupDescriptorPool();
        }


        private void CreatePipelineCache()
        {
            VkPipelineCacheCreateInfo pipelineCacheCreateInfo = new VkPipelineCacheCreateInfo
            {
                sType = VkStructureType.PipelineCacheCreateInfo,
            };
            vkCreatePipelineCache(NativeDevice.handle, &pipelineCacheCreateInfo, null, out _pipelineCache);
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

        private void SetupDescriptorSetLayout()
        {
            // Setup layout of descriptors used in this example
            // Basically connects the different shader stages to descriptors for binding uniform buffers, image samplers, etc.
            // So every shader binding should map to one descriptor set layout binding

            // Binding 0: Uniform buffer (Vertex shader)
            VkDescriptorSetLayoutBinding layoutBinding = new VkDescriptorSetLayoutBinding
            {
                descriptorType = VkDescriptorType.UniformBuffer,
                descriptorCount = 1,
                stageFlags = VkShaderStageFlags.Vertex,
                pImmutableSamplers = null
            };

            VkDescriptorSetLayoutCreateInfo descriptorLayout = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutCreateInfo,
                bindingCount = 1,
                pBindings = &layoutBinding
            };

            vkCreateDescriptorSetLayout(NativeDevice.handle, &descriptorLayout, null, out _descriptorSetLayout);

            // Create the Pipeline layout that is used to generate the rendering pipelines that are based on this descriptor set layout
            // In a more complex scenario you would have different Pipeline layouts for different descriptor set layouts that could be reused
            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
            {
                sType = VkStructureType.PipelineLayoutCreateInfo,
                pNext = null,
                setLayoutCount = 1
            };
            VkDescriptorSetLayout dsl = _descriptorSetLayout;
            pPipelineLayoutCreateInfo.pSetLayouts = &dsl;

            vkCreatePipelineLayout(NativeDevice.handle, &pPipelineLayoutCreateInfo, null, out _pipelineLayout);
        }


        public void SetupDescriptorSet(Buffer buffer)
        {
            // Allocate a new descriptor set from the global descriptor pool
            VkDescriptorSetAllocateInfo allocInfo = new VkDescriptorSetAllocateInfo
            {
                sType = VkStructureType.DescriptorSetAllocateInfo,
                descriptorPool = _descriptorPool,
                descriptorSetCount = 1
            };
            VkDescriptorSetLayout dsl = _descriptorSetLayout;
            allocInfo.pSetLayouts = &dsl;

            VkDescriptorSet descriptorSetptr;
            vkAllocateDescriptorSets(NativeDevice.handle, &allocInfo, &descriptorSetptr);
            _descriptorSet = descriptorSetptr;

            // Update the descriptor set determining the shader binding points
            // For every binding point used in a shader there needs to be one
            // descriptor set matching that binding point

            VkWriteDescriptorSet writeDescriptorSet = new VkWriteDescriptorSet()
            {
                sType = VkStructureType.WriteDescriptorSet
            };

            // Binding 0 : Uniform buffer
            writeDescriptorSet.dstSet = _descriptorSet;
            writeDescriptorSet.descriptorCount = 1;
            writeDescriptorSet.descriptorType = VkDescriptorType.UniformBuffer;
            VkDescriptorBufferInfo descriptor = new VkDescriptorBufferInfo
            {
                buffer = buffer.Handle,
                offset = 0,
                range = (ulong)buffer.SizeInBytes
            };
            writeDescriptorSet.pBufferInfo = &descriptor;
            // Binds this uniform buffer to binding point 0
            writeDescriptorSet.dstBinding = 0;

            vkUpdateDescriptorSets(NativeDevice.handle, 1, &writeDescriptorSet, 0, null);
        }





        private void CreateGraphicsPipeline(PipelineStateDescription description)
        {

            List<ShaderBytecode> shaders = description.Shaders;
            VkPipelineShaderStageCreateInfo* shaderStageCreateInfos = stackalloc VkPipelineShaderStageCreateInfo[shaders.Count];

            for (int i = 0; i < shaders.Count; i++)
            {
                shaderStageCreateInfos[i] = new VkPipelineShaderStageCreateInfo
                {
                    sType = VkStructureType.PipelineShaderStageCreateInfo,
                    pNext = null,
                    stage = (VkShaderStageFlags)shaders[i].Stage,
                    module = NativeDevice.LoadSpir_V_Shader(shaders[i].Data),
                    pName = Interop.String.ToPointer("main"),
                };
            }



            List<VertexInputAttribute> vertexAttribute = description.PipelineVertexInput.VertexAttributeDescriptions;
            VkVertexInputAttributeDescription* attributeDescr = stackalloc VkVertexInputAttributeDescription[vertexAttribute.Count];

            for (int i = 0; i < vertexAttribute.Count; i++)
            {
                attributeDescr[i] = new VkVertexInputAttributeDescription
                {
                    binding = (uint)vertexAttribute[i].Binding,
                    format = (VkFormat)vertexAttribute[i].Format,
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
                    inputRate = vertexBinding[i].InputRate is VertexInputRate.Vertex ? VkVertexInputRate.Vertex : VkVertexInputRate.Instance,
                    stride = (uint)vertexBinding[i].Stride
                };
            }




            VkPipelineVertexInputStateCreateInfo vertexInputStateCreate_info = new VkPipelineVertexInputStateCreateInfo()
            {
                sType = VkStructureType.PipelineVertexInputStateCreateInfo,
                pNext = null,
                flags = VkPipelineVertexInputStateCreateFlags.None,

                vertexBindingDescriptionCount = (uint)vertexBinding.Count,
                pVertexBindingDescriptions = vertexBindingDesc,

                vertexAttributeDescriptionCount = (uint)vertexAttribute.Count,
                pVertexAttributeDescriptions = attributeDescr,
            };



            VkPipelineInputAssemblyStateCreateInfo pipelineInputAssemblyStateCreateInfo = new VkPipelineInputAssemblyStateCreateInfo()
            {
                sType = VkStructureType.PipelineInputAssemblyStateCreateInfo,
                flags = 0,
                pNext = null,
            };

            if (PipelineStateDescription.InputAssemblyState != null)
            {
                pipelineInputAssemblyStateCreateInfo.primitiveRestartEnable = PipelineStateDescription.InputAssemblyState.PrimitiveRestartEnable;
                pipelineInputAssemblyStateCreateInfo.topology = VulkanConvert.ToPrimitiveType(PipelineStateDescription.InputAssemblyState.PrimitiveType);
            }
            else
            {
                pipelineInputAssemblyStateCreateInfo.primitiveRestartEnable = true;
                pipelineInputAssemblyStateCreateInfo.topology = VkPrimitiveTopology.PointList;
            }






            VkPipelineRasterizationStateCreateInfo rasterizerState = new VkPipelineRasterizationStateCreateInfo()
            {
                sType = VkStructureType.PipelineRasterizationStateCreateInfo,
                pNext = null,
            };

            if (PipelineStateDescription.RasterizationState != null)
            {
                rasterizerState.polygonMode = VulkanConvert.ToFillMode(PipelineStateDescription.RasterizationState.FillMode);
                rasterizerState.cullMode = VulkanConvert.ToCullMode(PipelineStateDescription.RasterizationState.CullMode);
                rasterizerState.frontFace = VulkanConvert.ToFrontFace(PipelineStateDescription.RasterizationState.FrontFace);
                rasterizerState.lineWidth = PipelineStateDescription.RasterizationState.LineWidth;
                rasterizerState.depthBiasEnable = PipelineStateDescription.RasterizationState.DepthBiasEnable;
                rasterizerState.depthClampEnable = PipelineStateDescription.RasterizationState.DepthClampEnable;

                if (PipelineStateDescription.RasterizationState.DepthBiasClamp != 0)
                {
                    rasterizerState.depthBiasClamp = PipelineStateDescription.RasterizationState.DepthBiasClamp;
                }

                if (PipelineStateDescription.RasterizationState.DepthBiasConstantFactor != 0)
                {
                    rasterizerState.depthBiasConstantFactor = PipelineStateDescription.RasterizationState.DepthBiasConstantFactor;
                }


            }
            else
            {
                rasterizerState.polygonMode = VkPolygonMode.Fill;
                rasterizerState.cullMode = VkCullModeFlags.None;
                rasterizerState.frontFace = VkFrontFace.CounterClockwise;
                rasterizerState.lineWidth = 1.0F;
            }


            VkPipelineMultisampleStateCreateInfo multisampleState_info = new VkPipelineMultisampleStateCreateInfo()
            {
                sType = VkStructureType.PipelineMultisampleStateCreateInfo,
                pNext = null,
                flags = 0,
                rasterizationSamples = (VkSampleCountFlags)PipelineStateDescription.MultisampleState.MultisampleCount,
                minSampleShading = 1f,
            };


            VkPipelineColorBlendAttachmentState colorBlendAttachementState = new VkPipelineColorBlendAttachmentState
            {
                colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A,
                blendEnable = false
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
                sType = VkStructureType.PipelineDynamicStateCreateInfo
            };
            dynamicState.pDynamicStates = dynamicStateEnables;
            dynamicState.dynamicStateCount = 2;


            VkGraphicsPipelineCreateInfo graphicsPipelineCI = new VkGraphicsPipelineCreateInfo()
            {
                sType = VkStructureType.GraphicsPipelineCreateInfo,
                pNext = null,
                stageCount = (uint)shaders.Count,
                pStages = shaderStageCreateInfos,
                pVertexInputState = &vertexInputStateCreate_info,
                pInputAssemblyState = &pipelineInputAssemblyStateCreateInfo,
                pRasterizationState = &rasterizerState,
                pMultisampleState = &multisampleState_info,
                pColorBlendState = &colorBlendState,
                layout = _pipelineLayout,
                renderPass = description.Framebuffer.renderPass,
                subpass = 0,
                pDepthStencilState = &depthStencilState,
                pDynamicState = &dynamicState
            };



            VkPipeline pipeline;
            vkCreateGraphicsPipelines(NativeDevice.handle, _pipelineCache, 1, &graphicsPipelineCI, null, &pipeline);
            graphicsPipeline = pipeline;


            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaderStageCreateInfos[i].module != VkShaderModule.Null)
                {
                    vkDestroyShaderModule(NativeDevice.handle, shaderStageCreateInfos[i].module, null);
                    //shaderStageCreateInfos[i] = *(VkPipelineShaderStageCreateInfo*)null;
                }
            }
            shaders.Clear();
        }


        public void CmdBindDescriptorSets(CommandBuffer buffer)
        {
            // Bind descriptor sets describing shader binding points
            VkDescriptorSet ds = _descriptorSet;
            vkCmdBindDescriptorSets(buffer.NativeCommandBuffer, VkPipelineBindPoint.Graphics, _pipelineLayout, 0, 1, &ds, 0, null);
        }


    }
}
