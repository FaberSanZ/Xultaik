// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	PipelineState.cs
=============================================================================*/



using SixLabors.ImageSharp.Formats;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
        internal VkPipelineLayout _pipelineLayout;

        public PipelineState(PipelineStateDescription description) : base(description.Framebuffer.NativeDevice)
        {
            PipelineStateDescription = description;
            Recreate();
        }

        public PipelineStateDescription PipelineStateDescription { get; set; }


        private void Recreate()
        {
            SetupDescriptorSetLayout();
            CreatePipelineLayout();

            CreatePipelineCache();



            CreateGraphicsPipeline(PipelineStateDescription);

        }

        private void SetupDescriptorSetLayout()
        {
            // Setup layout of descriptors used in this example
            // Basically connects the different shader stages to descriptors for binding uniform buffers, image samplers, etc.
            // So every shader binding should map to one descriptor set layout binding
            List<DescriptorSetLayout> Layouts = PipelineStateDescription.Layouts;

            VkDescriptorSetLayoutBinding* layoutBinding = stackalloc VkDescriptorSetLayoutBinding[Layouts.Count];

            for (int i = 0; i < Layouts.Count; i++)
            {
                layoutBinding[i] = new VkDescriptorSetLayoutBinding
                {
                    binding = (uint)Layouts[i].Binding,
                    descriptorCount = (uint)1, // TODO: descriptorCount
                    descriptorType = (VkDescriptorType)Layouts[i].Type, // TODO: TOVkDescriptorType
                    stageFlags = (VkShaderStageFlags)Layouts[i].Stage, // TODO: TOVkVkShaderStageFlags
                    pImmutableSamplers = null,
                };
            }

            VkDescriptorSetLayoutCreateInfo descriptorLayout = new VkDescriptorSetLayoutCreateInfo
            {
                sType = VkStructureType.DescriptorSetLayoutCreateInfo,
                bindingCount = (uint)Layouts.Count,
                pBindings = layoutBinding
            };

            vkCreateDescriptorSetLayout(NativeDevice.handle, &descriptorLayout, null, out _descriptorSetLayout);


        }


        internal void CreatePipelineLayout()
        {
            // Create the Pipeline layout that is used to generate the rendering pipelines that are based on this descriptor set layout
            // In a more complex scenario you would have different Pipeline layouts for different descriptor set layouts that could be reused
            VkPipelineLayoutCreateInfo pPipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
            {
                sType = VkStructureType.PipelineLayoutCreateInfo,
                pNext = null,
                setLayoutCount = 1
            };
            VkDescriptorSetLayout descriptorSetLayout = _descriptorSetLayout;
            pPipelineLayoutCreateInfo.pSetLayouts = &descriptorSetLayout;

            vkCreatePipelineLayout(NativeDevice.handle, &pPipelineLayoutCreateInfo, null, out _pipelineLayout);
        }








        private void CreatePipelineCache()
        {
            VkPipelineCacheCreateInfo pipelineCacheCreateInfo = new VkPipelineCacheCreateInfo
            {
                sType = VkStructureType.PipelineCacheCreateInfo,
            };
            vkCreatePipelineCache(NativeDevice.handle, &pipelineCacheCreateInfo, null, out _pipelineCache);
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
                layout = description.Layouts.Any() ? _pipelineLayout : _pipelineLayout, // TODO: _pipelineLayout
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

    }
}
