// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	CommandList.cs
=============================================================================*/


using Vortice.Mathematics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class CommandBuffer : GraphicsResource
    {

        internal uint imageIndex;
        internal VkCommandBuffer NativeCommandBuffer;

        public VkFence waitFences;




        public CommandBuffer(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            Recreate();
        }



        public void Recreate()
        {
            NativeCommandBuffer = NativeDevice.CreateCommandBufferPrimary();
            // Fences (Used to check draw command buffer completion)
            VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo()
            {
                sType = VkStructureType.FenceCreateInfo
            };
            // Create in signaled state so we don't wait on first render of each command buffer
            fenceCreateInfo.flags = VkFenceCreateFlags.Signaled;
            vkCreateFence(NativeDevice.handle, &fenceCreateInfo, null, out waitFences);

        }


        public void Begin()
        {
            // By setting timeout to UINT64_MAX we will always wait until the next image has been acquired or an actual error is thrown
            // With that we don't have to handle VK_NOT_READY
            vkAcquireNextImageKHR(NativeDevice.handle, NativeDevice.NativeSwapChain.SwapChain, ulong.MaxValue, NativeDevice.imageAvailableSemaphore, new VkFence(), out uint i);
            imageIndex = i;



            // Use a fence to wait until the command buffer has finished execution before using it again
            fixed (VkFence* ptrfence = &waitFences)
            {
                vkWaitForFences(NativeDevice.handle, 1, ptrfence, true, ulong.MaxValue);
                vkResetFences(NativeDevice.handle, 1, ptrfence);
            }


            VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo()
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.RenderPassContinue,
            };

            vkBeginCommandBuffer(NativeCommandBuffer, &beginInfo);



        }


        public void BeginFramebuffer(Framebuffer framebuffer, float r = 0, float g = .2f, float b = .4f, float a = 1.0f)
        {
            // Set clear values for all framebuffer attachments with loadOp set to clear
            // We use two attachments (color and depth) that are cleared at the start of the subpass and as such we need to set clear values for both
            VkClearValue* clearValues = stackalloc VkClearValue[2];
            clearValues[0].color = new VkClearColorValue(r, g, b, a);
            clearValues[1].depthStencil = new VkClearDepthStencilValue()
            {
                depth = 1.0f,
                stencil = 0
            };

            VkRenderPassBeginInfo renderPassBeginInfo = new VkRenderPassBeginInfo()
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderArea = new Rectangle()
                {
                    Height = NativeDevice.NativeParameters.BackBufferHeight,
                    Width = NativeDevice.NativeParameters.BackBufferWidth,
                    X = 0,
                    Y = 0,
                },
                renderPass = framebuffer.renderPass,
                clearValueCount = 2,
                pClearValues = clearValues,
                framebuffer = framebuffer.framebuffers[imageIndex], // Set target frame buffer
            };




            //VkRenderPassBeginInfo renderPassInfo = new VkRenderPassBeginInfo()
            //{
            //    sType = VkStructureType.RenderPassBeginInfo,
            //    renderPass = framebuffer.renderPass,
            //    framebuffer = framebuffer.framebuffers[imageIndex],
            //    renderArea = new Vortice.Mathematics.Rectangle()
            //    {
            //        Height = NativeDevice.NativeParameters.BackBufferHeight,
            //        Width = NativeDevice.NativeParameters.BackBufferWidth,
            //        //extent = new VkExtent2D((uint)NativeDevice.NativeParameters.BackBufferWidth, (uint)NativeDevice.NativeParameters.BackBufferHeight)
            //    },
            //};

            vkCmdBeginRenderPass(NativeCommandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);
        }


        public void Clear(float R, float G, float B, float A = 1.0f)
        {
            VkClearColorValue clearValue = new VkClearColorValue(R, G, B, A);

            VkImageSubresourceRange clearRange = new VkImageSubresourceRange()
            {
                aspectMask = VkImageAspectFlags.Color,
                baseMipLevel = 0,
                baseArrayLayer = 0,
                layerCount = 1,
                levelCount = 1
            };

            vkCmdClearColorImage(NativeCommandBuffer, NativeDevice.NativeSwapChain.Images[(int)imageIndex], VkImageLayout.ColorAttachmentOptimal, &clearValue, 1, &clearRange);
        }


        //public ulong srcOffset;
        //public ulong dstOffset;
        //public ulong size;

        public struct BufferCopy
        {
            public ulong SourceOffset;
            public ulong DestinationOffset;
            public ulong Size;
        }

        public void CopyBuffer(Buffer sourceBuffer, Buffer destinationBuffer, BufferCopy bufferCopy)
        {
            CopyBuffer(sourceBuffer.Handle, destinationBuffer.Handle, *(VkBufferCopy*)&bufferCopy);
        }


        internal void CopyBuffer(VkBuffer sourceBuffer, VkBuffer destinationBuffer, VkBufferCopy bufferCopy)
        {

            VkBufferMemoryBarrier* bufferBarriers = stackalloc VkBufferMemoryBarrier[2];
            bufferBarriers[0x0] = new VkBufferMemoryBarrier()
            {
                sType = VkStructureType.BufferMemoryBarrier,
                pNext = null,
            };

            bufferBarriers[0x1] = new VkBufferMemoryBarrier()
            {
                sType = VkStructureType.BufferMemoryBarrier,
                pNext = null,
            };
            //vkCmdPipelineBarrier()
            vkCmdCopyBuffer(NativeCommandBuffer, sourceBuffer, destinationBuffer, 1, &bufferCopy);

        }


        public void SetGraphicPipeline(PipelineState pipelineState)
        {
            vkCmdBindPipeline(NativeCommandBuffer, VkPipelineBindPoint.Graphics, pipelineState.graphicsPipeline);
        }


        public void SetComputePipeline(PipelineState pipelineState)
        {
            //vkCmdBindPipeline(NativeCommandBuffer, VkPipelineBindPoint.Compute, pipelineState.computesPipeline);
        }

        public void SetRayTracinPipeline(PipelineState pipelineState)
        {
            //vkCmdBindPipeline(NativeCommandBuffer, VkPipelineBindPoint.RayTracingNV, pipelineState.rayTracinPipeline);
        }


        public void SetScissor(int width, int height, int x, int y)
        {
            // Update dynamic scissor state
            Rectangle scissor = new Rectangle()
            {
                Width = width,
                Height = height,

                X = x,
                Y = y,
            };

            vkCmdSetScissor(NativeCommandBuffer, 0, 1, &scissor);
        }

        public void SetViewport(float Width, float Height, float X, float Y, float MinDepth = 0.0f, float MaxDepth = 1.0f)
        {
            Viewport Viewport = new Viewport()
            {
                Width = Width,
                Height = Height,

                X = X,
                Y = Y,

                MinDepth = MinDepth,
                MaxDepth = MaxDepth
            };

            vkCmdSetViewport(NativeCommandBuffer, 0, 1, &Viewport);
        }

        public void SetVertexBuffer(Buffer buffer, ulong offsets = 0)
        {
            fixed (VkBuffer* bufferptr = &buffer.Handle)
            {
                vkCmdBindVertexBuffers(NativeCommandBuffer, 0, 1, bufferptr, &offsets);
            }
        }

        public void SetVertexBuffers(Buffer[] buffers, ulong offsets = 0)
        {
            VkBuffer* buffer = stackalloc VkBuffer[buffers.Length];

            for (int i = 0; i < buffers.Length; i++)
            {
                buffer[i] = buffers[i].Handle;
            }

            //fixed(VkBuffer* bufferptr = &buffers[0].Handle)
            //{

            //}

            vkCmdBindVertexBuffers(NativeCommandBuffer, 0, 1, buffer, &offsets);
        }

        public void SetIndexBuffer(Buffer buffer, ulong offsets = 0, IndexType indexType = IndexType.Uint32)
        {
            if (buffer.Handle != VkBuffer.Null)
            {
                vkCmdBindIndexBuffer(NativeCommandBuffer, buffer.Handle, offsets, (VkIndexType)indexType);
            }
        }

        public void Draw(int vertexCount, int instanceCount, int firstVertex, int firstInstance)
        {
            vkCmdDraw(NativeCommandBuffer, (uint)vertexCount, (uint)instanceCount, (uint)firstVertex, (uint)firstInstance);
        }



        public void DrawIndexed(int indexCount, int instanceCount, int firstIndex, int vertexOffset, int firstInstance)
        {
            vkCmdDrawIndexed(NativeCommandBuffer, (uint)indexCount, (uint)instanceCount, (uint)firstIndex, vertexOffset, (uint)firstInstance);
        }



        public void Close()
        {
            CleanupRenderPass();
            vkEndCommandBuffer(NativeCommandBuffer);
        }


        internal unsafe void CleanupRenderPass()
        {
            vkCmdEndRenderPass(NativeCommandBuffer);
        }


        public void BindDescriptorSets(DescriptorSet descriptor)
        {
            // Bind descriptor sets describing shader binding points
            VkDescriptorSet descriptor_set = descriptor._descriptorSet;
            VkPipelineLayout _pipelineLayout = descriptor.PipelineState._pipelineLayout;

            vkCmdBindDescriptorSets(NativeCommandBuffer, VkPipelineBindPoint.Graphics, _pipelineLayout, 0, 1, &descriptor_set, 0, null);
        }



        public void Submit()
        {
            VkSemaphore signalSemaphore = NativeDevice.renderFinishedSemaphore;
            VkSemaphore waitSemaphore = NativeDevice.imageAvailableSemaphore;
            VkPipelineStageFlags waitStages = VkPipelineStageFlags.ColorAttachmentOutput;
            VkCommandBuffer commandBuffer = NativeCommandBuffer;


            VkSubmitInfo submitInfo = new VkSubmitInfo()
            {
                sType = VkStructureType.SubmitInfo,
                waitSemaphoreCount = 1,
                pWaitSemaphores = &waitSemaphore,
                pWaitDstStageMask = &waitStages,
                pNext = null,
                commandBufferCount = 1,
                pCommandBuffers = &commandBuffer,
                signalSemaphoreCount = 1,
                pSignalSemaphores = &signalSemaphore,
            };

            vkQueueSubmit(NativeDevice.nativeCommandQueue, 1, &submitInfo, waitFences);
        }


    }
}
