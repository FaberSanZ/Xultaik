// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	CommandList.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;
using Vulkan;
using Zeckoxe.Core;
using Zeckoxe.Mathematics;
using static Vulkan.VulkanNative;

namespace Zeckoxe.Graphics
{
    public unsafe class CommandList : GraphicsResource
    {

        internal uint imageIndex;
        internal VkCommandBuffer CommandBuffer;





        public CommandList(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            Recreate();
        }



        public void Recreate()
        {
            CommandBuffer = NativeDevice.CreateCommandBufferPrimary();
        }


        public void Begin()
        {
            // By setting timeout to UINT64_MAX we will always wait until the next image has been acquired or an actual error is thrown
            // With that we don't have to handle VK_NOT_READY
            uint i = 0; 
            vkAcquireNextImageKHR(NativeDevice.Device, NativeDevice.NativeSwapChain.SwapChain, ulong.MaxValue, NativeDevice.ImageAvailableSemaphore, new VkFence(), &i);
            imageIndex = i; 


            VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo()
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.RenderPassContinue,
            };

            vkBeginCommandBuffer(CommandBuffer, &beginInfo);
        }


        public void BeginFramebuffer(Framebuffer framebuffer)
        {

            VkRenderPassBeginInfo renderPassInfo = new VkRenderPassBeginInfo()
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderPass = framebuffer.NativeRenderPass,
                framebuffer = framebuffer.SwapChainFramebuffers[imageIndex],
                renderArea = new VkRect2D()
                {
                    extent = new VkExtent2D(NativeDevice.NativeParameters.BackBufferWidth, NativeDevice.NativeParameters.BackBufferHeight)
                },
            };

            vkCmdBeginRenderPass(CommandBuffer, &renderPassInfo, VkSubpassContents.Inline);
        }


        public void Clear(float R, float G, float B, float A = 1.0f)
        {
            VkClearColorValue clearValue = new VkClearColorValue(R, G, B);

            VkImageSubresourceRange clearRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1);

            vkCmdClearColorImage(CommandBuffer, NativeDevice.NativeSwapChain.Images[(int)imageIndex], VkImageLayout.ColorAttachmentOptimal, &clearValue, 1, &clearRange);
        }


        public void Clear(RawColor color)
        {
            VkClearColorValue clearValue = new VkClearColorValue(color.R, color.G, color.B, color.A);

            VkImageSubresourceRange clearRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1);

            vkCmdClearColorImage(CommandBuffer, NativeDevice.NativeSwapChain.Images[(int)imageIndex], VkImageLayout.ColorAttachmentOptimal, &clearValue, 1, &clearRange);
        }




        public void SetPipelineState(PipelineState pipelineState)
        {
            vkCmdBindPipeline(CommandBuffer, VkPipelineBindPoint.Graphics, pipelineState.graphicsPipeline);
        }


        public void DrawIndexed(int indexCount, int instanceCount, int firstIndex, int vertexOffset, int firstInstance)
        {

        }

        public void SetScissor(int width, int height, int x, int y)
        {
            // Update dynamic scissor state
            VkRect2D scissor = new VkRect2D();
            scissor.extent.width = (uint)width;
            scissor.extent.height = (uint)height;
            scissor.offset.x = x;
            scissor.offset.y = y;
            vkCmdSetScissor(CommandBuffer, 0, 1, &scissor);
        }

        public void SetViewport(float Width, float Height, float X, float Y, float MinDepth = 0.0f, float MaxDepth = 1.0f)
        {
            VkViewport Viewport = new VkViewport()
            {
                width = Width,

                height = Height,

                x = X,

                y = Y,

                minDepth = MinDepth,

                maxDepth = MaxDepth
            };

            vkCmdSetViewport(CommandBuffer, 0, 1, &Viewport);
        }

        public void SetVertexBuffer(Buffer buffer)
        {

        }

        public void SetIndexBuffer(Buffer buffer)
        {

        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, int firstInstance)
        {
            vkCmdDraw(CommandBuffer, vertexCount, instanceCount, 0, 0);

        }




        public void End()
        {
            vkEndCommandBuffer(CommandBuffer);
        }


        public void EndFramebuffer()
        {
            vkCmdEndRenderPass(CommandBuffer);

        }


        public void Submit()
        {
            VkSemaphore signalSemaphore = NativeDevice.RenderFinishedSemaphore;
            VkSemaphore waitSemaphore = NativeDevice.ImageAvailableSemaphore;
            VkPipelineStageFlags waitStages = VkPipelineStageFlags.ColorAttachmentOutput;
            VkCommandBuffer commandBuffer = CommandBuffer;


            VkSubmitInfo submitInfo = new VkSubmitInfo()
            {
                sType = VkStructureType.SubmitInfo,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &waitSemaphore,
                pWaitDstStageMask = &waitStages,
                commandBufferCount = 1,
                pCommandBuffers = &commandBuffer,
                signalSemaphoreCount = 1,
                pSignalSemaphores = &signalSemaphore,
            };

            vkQueueSubmit(NativeDevice.NativeCommandQueue, 1, &submitInfo, VkFence.Null);
        }


    }
}
