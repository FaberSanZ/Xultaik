// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineState.cs
=============================================================================*/



using System.Collections.Generic;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class PipelineState : GraphicsResource
    {

        internal VkPipelineLayout pipelineLayout;
        internal VkPipeline graphicsPipeline;

        public PipelineState(PipelineStateDescription description) : base(description.Framebuffer.NativeDevice)
        {
            PipelineStateDescription = description;
            Recreate();
        }

        public PipelineStateDescription PipelineStateDescription { get; set; }



        private void Recreate()
        {
            CreatePipelineLayout();

            CreateGraphicsPipeline(PipelineStateDescription);
        }


        private void CreateGraphicsPipeline(PipelineStateDescription description)
        {

            uint stageCount = 0;
            bool Vertex = PipelineStateDescription.Vertex.Data.Length != 0;
            bool Fragment = PipelineStateDescription.Fragment.Data.Length != 0;
            VkShaderModule vertexShader = VkShaderModule.Null;
            VkShaderModule fragmentShader = VkShaderModule.Null;


            if (Vertex)
            {
                stageCount++;
                vertexShader = NativeDevice.LoadSPIR_V_Shader(PipelineStateDescription.Vertex.Data);

            }

            if (Fragment)
            {
                stageCount++;
                fragmentShader = NativeDevice.LoadSPIR_V_Shader(PipelineStateDescription.Fragment.Data);
            }







            VkPipelineShaderStageCreateInfo vertCreateInfo = new VkPipelineShaderStageCreateInfo()
            {
                sType = VkStructureType.PipelineShaderStageCreateInfo,
                pNext = null,
                stage = VkShaderStageFlags.Vertex,
                module = vertexShader,
                pName = Interop.String.ToPointer("main"),
            };


            VkPipelineShaderStageCreateInfo fragCreateInfo = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.PipelineShaderStageCreateInfo,
                pNext = null,
                flags = 0,
                stage = VkShaderStageFlags.Fragment,
                module = fragmentShader,
                pName = Interop.String.ToPointer("main"),
            };




            VkPipelineShaderStageCreateInfo* shaderStageCreateInfos = stackalloc VkPipelineShaderStageCreateInfo[(int)stageCount];


            if (Vertex)
            {
                shaderStageCreateInfos[0] = vertCreateInfo;
            }

            if (Fragment)
            {
                shaderStageCreateInfos[1] = fragCreateInfo;
            }






            var VertexAttributeDescriptions = description.PipelineVertexInput.VertexAttributeDescriptions;
            var VertexBindingDescriptions = description.PipelineVertexInput.VertexBindingDescriptions;

            VkVertexInputAttributeDescription* attributeDescr = stackalloc VkVertexInputAttributeDescription[VertexAttributeDescriptions.Count];
            VkVertexInputBindingDescription* vertexBindingDesc = stackalloc VkVertexInputBindingDescription[VertexBindingDescriptions.Count];



            for (int i = 0; i < VertexAttributeDescriptions.Count; i++)
            {
                attributeDescr[i] = new VkVertexInputAttributeDescription
                {
                    binding = (uint)VertexAttributeDescriptions[i].Binding,
                    format = (VkFormat)VertexAttributeDescriptions[i].Format,
                    location = (uint)VertexAttributeDescriptions[i].Location,
                    offset = (uint)VertexAttributeDescriptions[i].Offset,
                };
            }


            for (int i = 0; i < VertexBindingDescriptions.Count; i++)
            {
                vertexBindingDesc[i] = new VkVertexInputBindingDescription
                {
                    binding = (uint)VertexBindingDescriptions[i].Binding,
                    inputRate = VertexBindingDescriptions[i].InputRate is VertexInputRate.Vertex ? VkVertexInputRate.Vertex : VkVertexInputRate.Instance,
                    stride = (uint)VertexBindingDescriptions[i].Stride
                };
            }

            VkPipelineVertexInputStateCreateInfo vertexInputStateCreate_info = new VkPipelineVertexInputStateCreateInfo()
            {
                sType = VkStructureType.PipelineVertexInputStateCreateInfo,
                pNext = null,
                flags = VkPipelineVertexInputStateCreateFlags.None,

                vertexBindingDescriptionCount = (uint)VertexBindingDescriptions.Count,
                pVertexBindingDescriptions = vertexBindingDesc,

                vertexAttributeDescriptionCount = (uint)VertexAttributeDescriptions.Count,
                pVertexAttributeDescriptions = attributeDescr,
            };



            // PipelineInputAssemblyStateCreateInfo -------------
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
            // PipelineInputAssemblyStateCreateInfo -------------






            VkPipelineRasterizationStateCreateInfo rasterizerState = new VkPipelineRasterizationStateCreateInfo()
            {
                sType = VkStructureType.PipelineRasterizationStateCreateInfo,
                pNext = (void*)null,
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
                rasterizerState.frontFace = VkFrontFace.Clockwise;
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



            VkGraphicsPipelineCreateInfo graphicsPipelineCI = new VkGraphicsPipelineCreateInfo()
            {
                sType = VkStructureType.GraphicsPipelineCreateInfo,
                pNext = (void*)null,
                stageCount = stageCount,
                pStages = shaderStageCreateInfos,
                pVertexInputState = &vertexInputStateCreate_info,
                pInputAssemblyState = &pipelineInputAssemblyStateCreateInfo,
                pRasterizationState = &rasterizerState,
                pMultisampleState = &multisampleState_info,
                pColorBlendState = &colorBlendState,
                layout = pipelineLayout,
                renderPass = description.Framebuffer.renderPass,
                subpass = 0,
            };



            VkPipeline pipeline;
            vkCreateGraphicsPipelines(NativeDevice.handle, /*new VkPipelineCache(0)*/ VkPipelineCache.Null, 1, &graphicsPipelineCI, null, &pipeline);
            graphicsPipeline = pipeline;
        }

        internal void CreatePipelineLayout()
        {
            VkPipelineLayoutCreateInfo pipelineLayoutCI = new VkPipelineLayoutCreateInfo()
            {
                sType = VkStructureType.PipelineLayoutCreateInfo,
                pNext = null,
                setLayoutCount = 0,
                flags = VkPipelineLayoutCreateFlags.None,
            };

            vkCreatePipelineLayout(NativeDevice.handle, &pipelineLayoutCI, null, out VkPipelineLayout vkpipelineLayout);
            pipelineLayout = vkpipelineLayout;
        }


    }
}
