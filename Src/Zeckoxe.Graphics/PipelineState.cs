// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineState.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Zeckoxe.Core;
using Vulkan;
using static Vulkan.VulkanNative;

namespace Zeckoxe.Graphics
{
    public unsafe class PipelineState : GraphicsResource
    {
        public PipelineStateDescription PipelineStateDescription { get; set; }


        internal VkPipelineLayout pipelineLayout;
        internal VkPipeline graphicsPipeline;

        public PipelineState(PipelineStateDescription description) : base(description.Framebuffer.NativeDevice)
        {
            PipelineStateDescription = description;
            Recreate();
        }




        private void Recreate()
        {
            CreatePipelineLayout();

            CreateGraphicsPipeline(PipelineStateDescription);
        }



        public void CreatePipelineLayout()
        {

        }



        internal void CreateGraphicsPipeline(PipelineStateDescription description)
        {

            uint stageCount = 0;
            bool Vertex = PipelineStateDescription.Vertex.Data.Length != 0;
            bool Fragment = PipelineStateDescription.Fragment.Data.Length != 0;
            bool Pixel =  PipelineStateDescription.Pixel.Data.Length != 0;
            bool Compute = PipelineStateDescription.Compute.Data.Length != 0;
            VkShaderModule vertexShader = VkShaderModule.Null;
            VkShaderModule fragmentShader = VkShaderModule.Null;
            VkShaderModule pixelShader = VkShaderModule.Null;
            VkShaderModule computeShader = VkShaderModule.Null;


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

            if (Pixel)
            {
                stageCount++;
                pixelShader = NativeDevice.LoadSPIR_V_Shader(PipelineStateDescription.Pixel.Data);
            }


            if (Compute)
            {
                stageCount++;
                computeShader = NativeDevice.LoadSPIR_V_Shader(PipelineStateDescription.Compute.Data);
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
                module = fragmentShader == VkShaderModule.Null ? pixelShader : fragmentShader,
                pName = Interop.String.ToPointer("main"),
            };


            VkPipelineShaderStageCreateInfo ComputeCreateInfo = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.PipelineShaderStageCreateInfo,
                pNext = null,
                flags = 0,
                stage = VkShaderStageFlags.Compute,
                module = computeShader,
                pName = Interop.String.ToPointer("main"),
            };







            VkPipelineShaderStageCreateInfo* shaderStageCreateInfos = stackalloc VkPipelineShaderStageCreateInfo[(int)stageCount];


            if (Vertex)
                shaderStageCreateInfos[0] = vertCreateInfo;
            
            if (Fragment || Pixel)
                shaderStageCreateInfos[1] = fragCreateInfo;
            // is Compute?
            if (Compute)
                shaderStageCreateInfos[2] = ComputeCreateInfo;



            VkPipelineVertexInputStateCreateInfo vertexInputStateCI = VkPipelineVertexInputStateCreateInfo.New();
            //var vertexBindingDesc = Vertex.GetBindingDescription();
            vertexInputStateCI.vertexBindingDescriptionCount = 0;
            //vertexInputStateCI.pVertexBindingDescriptions = Interop.Struct.AllocToPointer(ref vertexBindingDesc);


            vertexInputStateCI.vertexAttributeDescriptionCount = 0;
            //vertexInputStateCI.pVertexAttributeDescriptions = Interop.Struct.AllocToPointer(Vertex.GetAttributeDescriptions());
            //vertexInputStateCI.pVertexAttributeDescriptions = Interop.Struct.AllocToPointer(Vertex.GetAttributeDescriptions().AsSpan());





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






            VkPipelineRasterizationStateCreateInfo rasterizerStateCI = VkPipelineRasterizationStateCreateInfo.New();
            rasterizerStateCI.cullMode = VkCullModeFlags.None;
            rasterizerStateCI.polygonMode = VkPolygonMode.Fill;
            rasterizerStateCI.lineWidth = 3.5f;
            rasterizerStateCI.frontFace = VkFrontFace.CounterClockwise;

            VkPipelineMultisampleStateCreateInfo multisampleStateCI = VkPipelineMultisampleStateCreateInfo.New();
            multisampleStateCI.rasterizationSamples = VkSampleCountFlags.Count1;
            multisampleStateCI.minSampleShading = 1f;

            VkPipelineColorBlendAttachmentState colorBlendAttachementState = new VkPipelineColorBlendAttachmentState();
            colorBlendAttachementState.colorWriteMask = VkColorComponentFlags.R | VkColorComponentFlags.G | VkColorComponentFlags.B | VkColorComponentFlags.A;
            colorBlendAttachementState.blendEnable = false;

            VkPipelineColorBlendStateCreateInfo colorBlendStateCI = VkPipelineColorBlendStateCreateInfo.New();
            colorBlendStateCI.attachmentCount = 1;
            colorBlendStateCI.pAttachments = &colorBlendAttachementState;

            //VkDescriptorSetLayout dsl = _descriptoSetLayout;
            VkPipelineLayoutCreateInfo pipelineLayoutCI = VkPipelineLayoutCreateInfo.New();
            pipelineLayoutCI.setLayoutCount = 0;
            //pipelineLayoutCI.pSetLayouts = &dsl;
            vkCreatePipelineLayout(NativeDevice.Device, ref pipelineLayoutCI, null, out pipelineLayout);

            VkGraphicsPipelineCreateInfo graphicsPipelineCI = VkGraphicsPipelineCreateInfo.New();
            graphicsPipelineCI.stageCount = stageCount;
            graphicsPipelineCI.pStages = shaderStageCreateInfos;

            graphicsPipelineCI.pVertexInputState = &vertexInputStateCI;
            graphicsPipelineCI.pInputAssemblyState = &pipelineInputAssemblyStateCreateInfo;
            graphicsPipelineCI.pRasterizationState = &rasterizerStateCI;
            graphicsPipelineCI.pMultisampleState = &multisampleStateCI;
            graphicsPipelineCI.pColorBlendState = &colorBlendStateCI;
            graphicsPipelineCI.layout = pipelineLayout;
            graphicsPipelineCI.renderPass = description.Framebuffer.NativeRenderPass;
            graphicsPipelineCI.subpass = 0;

            vkCreateGraphicsPipelines(NativeDevice.Device, VkPipelineCache.Null, 1, ref graphicsPipelineCI, null, out graphicsPipeline);
        }


    }
}
