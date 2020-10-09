// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	CommandList.cs
=============================================================================*/


using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public enum CommandBufferType
    {
        Generic,

        AsyncGraphics,

        AsyncCompute,

        AsyncTransfer,

        Count
    }

    //TODO: CommandBufferType
    public unsafe class CommandBuffer : GraphicsResource
    {

        internal uint imageIndex;
        internal VkCommandBuffer handle;
        internal VkFence waitFences; // TODO: VkFence -> Fence(0)



        public CommandBuffer(GraphicsDevice graphicsDevice, CommandBufferType type) : base(graphicsDevice)
        {
            Type = type;

            Recreate();
        }

        public CommandBufferType Type { get; set; }


        public void Recreate()
        {
            handle = NativeDevice.CreateCommandBufferPrimary();

            // Fences (Used to check draw command buffer completion)
            VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo()
            {
                sType = VkStructureType.FenceCreateInfo
            };

            // Create in signaled state so we don't wait on first render of each command buffer
            fenceCreateInfo.flags = VkFenceCreateFlags.Signaled;
            vkCreateFence(NativeDevice.handle, &fenceCreateInfo, null, out waitFences);

        }

        public CommandBufferType GetPhysicalQueueType(CommandBufferType type)
        {
            if (type != CommandBufferType.AsyncGraphics)
            {
                return type;
            }

            else
            {
                if (NativeDevice.GraphicsFamily == NativeDevice.ComputeFamily && NativeDevice.queueFamilyProperties /*graphics_queue*/ != NativeDevice.queueFamilyProperties /*compute_queue*/)
                {
                    return CommandBufferType.AsyncCompute;
                }
                else
                {
                    return CommandBufferType.Generic;
                }
            }
        }

        public void Begin(SwapChain swapChain)
        {
            // By setting timeout to UINT64_MAX we will always wait until the next image has been acquired or an actual error is thrown
            // With that we don't have to handle VK_NOT_READY
            vkAcquireNextImageKHR(NativeDevice.handle, swapChain.handle, ulong.MaxValue, NativeDevice.imageAvailableSemaphore, new VkFence(), out uint i);
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

            vkBeginCommandBuffer(handle, &beginInfo);



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
                renderArea = new VkRect2D()
                {
                    extent = new VkExtent2D()
                    {
                        height = (uint)NativeDevice.NativeParameters.BackBufferHeight,
                        width = (uint)NativeDevice.NativeParameters.BackBufferWidth,
                    },
                    offset = new VkOffset2D()
                    {
                        x = 0,
                        y = 0,
                    }
                },
                renderPass = framebuffer.renderPass,
                clearValueCount = 2,
                pClearValues = clearValues,
                framebuffer = framebuffer.framebuffers[imageIndex], // Set target frame buffer
            };

            vkCmdBeginRenderPass(handle, &renderPassBeginInfo, VkSubpassContents.Inline);
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

            //vkCmdClearColorImage(NativeCommandBuffer, NativeDevice.SwapChain.Images[(int)imageIndex], VkImageLayout.ColorAttachmentOptimal, &clearValue, 1, &clearRange);
        }


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
            vkCmdCopyBuffer(handle, sourceBuffer, destinationBuffer, 1, &bufferCopy);

        }

        // TODO: ALL_GPUS  
        internal void set_current_gpu(int gpu_index)
        {
            //if (NativeDevice.device_count > 1)
            //{
            //    if (gpu_index == ALL_GPUS)
            //        vkCmdSetDeviceMaskKHR(handle, (1 << NativeDevice.device_count) - 1);
            //    else
            //        qvkCmdSetDeviceMaskKHR(handle, 1 << gpu_index);
            //}
        }


        public void SetCullMode(CullMode mode)
        {
            vkCmdSetCullModeEXT(handle, mode.ToCullMode());
        }

        public void SetLineWidth(float lineWidth)
        {
            vkCmdSetLineWidth(handle, lineWidth);
        }

        public void SetFrontFace(FrontFace frontFace)
        {
            vkCmdSetFrontFaceEXT(handle, frontFace.ToFrontFace());
        }

        public void SetPrimitiveTopology(PrimitiveType type)
        {
            vkCmdSetPrimitiveTopologyEXT(handle, type.ToPrimitiveType());
        }


        public void SetGraphicPipeline(PipelineState pipelineState)
        {
            vkCmdBindPipeline(handle, VkPipelineBindPoint.Graphics, pipelineState.graphicsPipeline);
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
            VkRect2D scissor = new VkRect2D()
            {
                extent = new VkExtent2D()
                {
                    height = (uint)width,
                    width = (uint)height,
                },
                offset = new VkOffset2D()
                {
                    x = x,
                    y = y,
                }
            };

            vkCmdSetScissor(handle, 0, 1, &scissor);
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

            vkCmdSetViewport(handle, 0, 1, &Viewport);
        }

        public void SetVertexBuffer(Buffer buffer, ulong offsets = 0)
        {
            fixed (VkBuffer* bufferptr = &buffer.Handle)
            {
                vkCmdBindVertexBuffers(handle, 0, 1, bufferptr, &offsets);
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

            vkCmdBindVertexBuffers(handle, 0, 1, buffer, &offsets);
        }

        public void SetIndexBuffer(Buffer buffer, ulong offsets = 0, IndexType indexType = IndexType.Uint32)
        {
            if (buffer.Handle != VkBuffer.Null)
            {
                vkCmdBindIndexBuffer(handle, buffer.Handle, offsets, (VkIndexType)indexType);
            }
        }

        public void Draw(int vertexCount, int instanceCount, int firstVertex, int firstInstance)
        {
            vkCmdDraw(handle, (uint)vertexCount, (uint)instanceCount, (uint)firstVertex, (uint)firstInstance);
        }



        public void DrawIndexed(int indexCount, int instanceCount, int firstIndex, int vertexOffset, int firstInstance)
        {
            vkCmdDrawIndexed(handle, (uint)indexCount, (uint)instanceCount, (uint)firstIndex, vertexOffset, (uint)firstInstance);
        }



        public void Close()
        {
            CleanupRenderPass();
            vkEndCommandBuffer(handle);
        }


        internal unsafe void CleanupRenderPass()
        {
            vkCmdEndRenderPass(handle);
        }


        public void BindDescriptorSets(DescriptorSet descriptor)
        {
            // Bind descriptor sets describing shader binding points
            VkDescriptorSet descriptor_set = descriptor._descriptorSet;
            VkPipelineLayout _pipelineLayout = descriptor.PipelineState._pipelineLayout;

            vkCmdBindDescriptorSets(handle, VkPipelineBindPoint.Graphics, _pipelineLayout, 0, 1, &descriptor_set, 0, null);
        }



        public void Submit()
        {
            VkSemaphore signalSemaphore = NativeDevice.renderFinishedSemaphore;
            VkSemaphore waitSemaphore = NativeDevice.imageAvailableSemaphore;
            VkPipelineStageFlags waitStages = VkPipelineStageFlags.ColorAttachmentOutput;
            VkCommandBuffer commandBuffer = handle;


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
