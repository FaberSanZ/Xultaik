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
        VkPipelineLayout pipelineLayout;
        internal VkPipeline graphicsPipeline;

        ShaderBytecode VertShaderbyteCode { get; set; }

        ShaderBytecode FragShaderbyteCode { get; set; }

        string[] FileShaders { get; set; }

        public PipelineState(ShaderBytecode[] fileShaders, Framebuffer framebuffer) : base(framebuffer.NativeDevice)
        {
            Recreate(fileShaders, framebuffer);
        }




        private void Recreate(ShaderBytecode[] fileShaders, Framebuffer framebuffer)
        {
            CreatePipelineLayout();

            CreateGraphicsPipeline(fileShaders, framebuffer);
        }



        public void CreatePipelineLayout()
        {

        }

        private VkShaderModule CreateShader(byte[] bytecode)
        {
            VkShaderModuleCreateInfo smci = VkShaderModuleCreateInfo.New();
            fixed (byte* byteCodePtr = bytecode)
            {
                smci.pCode = (uint*)byteCodePtr;
                smci.codeSize = new UIntPtr((uint)bytecode.Length);
                vkCreateShaderModule(NativeDevice.Device, ref smci, null, out VkShaderModule module);
                return module;
            }
        }


        public void CreateGraphicsPipeline(ShaderBytecode[] fileShaders, Framebuffer framebuffer)
        {

            VkShaderModule vertexShader = NativeDevice.LoadSPIR_V_Shader(fileShaders[0]);
            VkShaderModule fragmentShader = NativeDevice.LoadSPIR_V_Shader(fileShaders[1]);

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


            VkPipelineShaderStageCreateInfo* shaderStageCreateInfos = stackalloc VkPipelineShaderStageCreateInfo[2];

            shaderStageCreateInfos[0] = vertCreateInfo;
            shaderStageCreateInfos[1] = fragCreateInfo;

            VkPipelineVertexInputStateCreateInfo vertexInputStateCI = VkPipelineVertexInputStateCreateInfo.New();
            //var vertexBindingDesc = Vertex.GetBindingDescription();
            vertexInputStateCI.vertexBindingDescriptionCount = 0;
            //vertexInputStateCI.pVertexBindingDescriptions = Interop.Struct.AllocToPointer(ref vertexBindingDesc);


            vertexInputStateCI.vertexAttributeDescriptionCount = 0;
            //vertexInputStateCI.pVertexAttributeDescriptions = Interop.Struct.AllocToPointer(Vertex.GetAttributeDescriptions());
            //vertexInputStateCI.pVertexAttributeDescriptions = Interop.Struct.AllocToPointer(Vertex.GetAttributeDescriptions().AsSpan());

            VkPipelineInputAssemblyStateCreateInfo inputAssemblyCI = VkPipelineInputAssemblyStateCreateInfo.New();
            inputAssemblyCI.primitiveRestartEnable = false;
            inputAssemblyCI.topology = VkPrimitiveTopology.TriangleList;


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
            graphicsPipelineCI.stageCount = 2;
            graphicsPipelineCI.pStages = shaderStageCreateInfos;

            graphicsPipelineCI.pVertexInputState = &vertexInputStateCI;
            graphicsPipelineCI.pInputAssemblyState = &inputAssemblyCI;
            graphicsPipelineCI.pRasterizationState = &rasterizerStateCI;
            graphicsPipelineCI.pMultisampleState = &multisampleStateCI;
            graphicsPipelineCI.pColorBlendState = &colorBlendStateCI;
            graphicsPipelineCI.layout = pipelineLayout;
            graphicsPipelineCI.renderPass = framebuffer.NativeRenderPass;
            graphicsPipelineCI.subpass = 0;

            vkCreateGraphicsPipelines(NativeDevice.Device, VkPipelineCache.Null, 1, ref graphicsPipelineCI, null, out graphicsPipeline);
        }


    }
}
