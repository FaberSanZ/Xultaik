// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    public unsafe class CommandBuffer : GraphicsResource, IDisposable
    {

        internal uint imageIndex;
        internal VkCommandBuffer handle;


        internal VkCommandPool cmd_command_pool;
        private bool set_render_pass;

        public CommandBuffer(Device graphicsDevice, CommandBufferType type) : base(graphicsDevice)
        {
            Type = type;


            Recreate();

            WaitFence = new(NativeDevice, true);
        }

        public CommandBufferType Type { get; set; }
        public Fence WaitFence { get; set; }

        public void Recreate()
        {

            // TODO: Reuse the same VkCommandPool only for transfer, compute and graphics use an independent one per thread

            switch (Type)
            {
                case CommandBufferType.Generic:
                    cmd_command_pool = NativeDevice.create_command_pool(NativeDevice.GraphicsFamily);
                    break;

                case CommandBufferType.AsyncGraphics:
                    cmd_command_pool = NativeDevice.create_command_pool(NativeDevice.GraphicsFamily);
                    break;

                case CommandBufferType.AsyncCompute:
                    cmd_command_pool = NativeDevice.create_command_pool(NativeDevice.ComputeFamily);
                    break;

                case CommandBufferType.AsyncTransfer:
                    cmd_command_pool = NativeDevice.transfer_cmd_pool;
                    break;

                case CommandBufferType.Count:
                    cmd_command_pool = NativeDevice.create_command_pool(NativeDevice.GraphicsFamily);
                    break;
            }

            handle = NativeDevice.create_command_buffer_primary(cmd_command_pool);


        }


        public void Begin()
        {
            BeginRenderPassContinue();

            WaitFence.Wait();
            WaitFence.Reset();
        }


        public void BeginFramebuffer(Framebuffer framebuffer, float r = 0, float g = .2f, float b = .4f, float a = 1.0f)
        {
            imageIndex = framebuffer.SwapChain.AcquireNextImage();

            // Set clear values for all framebuffer attachments with loadOp set to clear
            // We use two attachments (color and depth) that are cleared at the start of the subpass and as such we need to set clear values for both
            BeginRenderPassInline(framebuffer, r, g, b, a);
        }


        public void BeginRenderPassSecondaryCommandBuffers(Framebuffer framebuffer, float r, float g, float b, float a)
        {
            VkClearValue* clearValues = stackalloc VkClearValue[2];
            clearValues[0].color = new(r, g, b, a);
            clearValues[1].depthStencil = new(1, 0);

            int h = framebuffer.SwapChain.Height;
            int w = framebuffer.SwapChain.Width;
            int x = 0;
            int y = 0;

            VkRenderPassBeginInfo renderPassBeginInfo = new()
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderArea = new(x, y, w, h),

                renderPass = framebuffer.renderPass,
                clearValueCount = 2,
                pClearValues = clearValues,
                framebuffer = framebuffer.framebuffers[imageIndex], // Set target frame buffer
            };

            vkCmdBeginRenderPass(handle, &renderPassBeginInfo, VkSubpassContents.SecondaryCommandBuffers);
            set_render_pass = true;
        }


        public void BeginRenderPassInline(Framebuffer framebuffer, float r, float g, float b, float a)
        {
            VkClearValue* clearValues = stackalloc VkClearValue[2];
            clearValues[0].color = new(r, g, b, a);
            clearValues[1].depthStencil = new(1, 0);

            int h = framebuffer.SwapChain.Height;
            int w = framebuffer.SwapChain.Width;
            int x = 0;
            int y = 0;

            VkRenderPassBeginInfo renderPassBeginInfo = new()
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderArea = new(x, y, w, h),
                pNext = null,
                renderPass = framebuffer.renderPass,
                clearValueCount = 2,
                pClearValues = clearValues,
                framebuffer = framebuffer.framebuffers[imageIndex], // Set target frame buffer
            };

            vkCmdBeginRenderPass(handle, &renderPassBeginInfo, VkSubpassContents.Inline);
        }


        public void Clear(float R, float G, float B, float A = 1.0f)
        {
            VkClearColorValue clearValue = new(R, G, B, A);

            VkImageSubresourceRange clearRange = new()
            {
                aspectMask = VkImageAspectFlags.Color,
                baseMipLevel = 0,
                baseArrayLayer = 0,
                layerCount = 1,
                levelCount = 1
            };

            //vkCmdClearColorImage(NativeCommandBuffer, NativeDevice.SwapChain.Images[(int)imageIndex], VkImageLayout.ColorAttachmentOptimal, &clearValue, 1, &clearRange);
        }



        public void FillBuffer(Buffer dst, int value)
        {
            fill_buffer(dst.handle, (uint)value, 0, WholeSize);
        }

        internal void fill_buffer(VkBuffer dst, uint value, ulong offset, ulong size)
        {
            vkCmdFillBuffer(handle, dst, offset, size, value);
        }


        internal void copy_buffer(Buffer dst, ulong dst_offset, Buffer src, ulong src_offset, ulong size)
        {
            VkBufferCopy region = new()
            {
                size = size,
                srcOffset = src_offset,
                dstOffset = dst_offset,
            };

            vkCmdCopyBuffer(handle, src.handle, dst.handle, 1, &region);
        }

        public void CopyBuffer(Buffer dst, Buffer src)
        {
            if (dst.size == src.size)
            {
                // TODO: CopyBuffer - Assert
            }
            copy_buffer(dst, 0, src, 0, (uint)dst.size);
        }


        internal void copy_buffer(Buffer dst, Buffer src, VkBufferCopy* copies, uint count)
        {
            vkCmdCopyBuffer(handle, src.handle, dst.handle, count, copies);
        }


        internal void Copy_Buffer(VkBuffer sourceBuffer, VkBuffer destinationBuffer, VkBufferCopy bufferCopy)
        {

            VkBufferMemoryBarrier* bufferBarriers = stackalloc VkBufferMemoryBarrier[2];
            bufferBarriers[0x0] = new()
            {
                sType = VkStructureType.BufferMemoryBarrier,
                pNext = null,
            };

            bufferBarriers[0x1] = new()
            {
                sType = VkStructureType.BufferMemoryBarrier,
                pNext = null,
            };
            //vkCmdPipelineBarrier()
            vkCmdCopyBuffer(handle, sourceBuffer, destinationBuffer, 1, &bufferCopy);

        }

        private void copy_image(Image dst, Image src, VkOffset3D dst_offset,
                        VkOffset3D src_offset, VkExtent3D extent,
                        VkImageSubresourceLayers dst_subresource,
                        VkImageSubresourceLayers src_subresource)
        {

            VkImageCopy region = new()
            {
                dstOffset = dst_offset,
                srcOffset = src_offset,
                extent = extent,
                srcSubresource = src_subresource,
                dstSubresource = dst_subresource,
            };


            vkCmdCopyImage(handle, src.handle, src.get_layout(VkImageLayout.TransferSrcOptimal), dst.handle, dst.get_layout(VkImageLayout.TransferDstOptimal), 1, &region);
        }


        internal void copy_image(Image dst, Image src)
        {
            VkImageCopy* regions = stackalloc VkImageCopy[32];

            int levels = src.Description.MipLevels;


            for (uint i = 0; i < levels; i++)
            {
                regions[i] = new()
                {
                    extent = new(src.Width, src.Height, src.Depth),

                    dstSubresource = new()
                    {
                        mipLevel = i,
                        aspectMask = VulkanConvert.format_to_aspect_mask(dst.Format),
                        layerCount = dst.layers,
                    },
                    srcSubresource = new()
                    {
                        mipLevel = i,
                        aspectMask = VulkanConvert.format_to_aspect_mask(src.Format),
                        layerCount = src.layers,
                    }
                };


                if (regions[i].srcSubresource.aspectMask == regions[i].dstSubresource.aspectMask)
                {
                    // TODO: copy_image - Assert
                }
            }

            vkCmdCopyImage(handle, src.handle, src.get_layout(VkImageLayout.TransferSrcOptimal), dst.handle, dst.get_layout(VkImageLayout.TransferDstOptimal), (uint)levels, regions);
        }


        public void CopyTexture(Image dst, Image src)
        {
            copy_image(dst, src);
        }


        internal void copy_buffer_to_image(VkImage image, VkBuffer buffer, uint num_blits, VkBufferImageCopy* blits, VkImageLayout layout)
        {
            vkCmdCopyBufferToImage(handle, buffer, image, layout, num_blits, blits);
        }

        internal void copy_buffer_to_image(Image image, Buffer buffer, uint num_blits, VkBufferImageCopy* blits)
        {
            vkCmdCopyBufferToImage(handle, buffer.handle, image.handle, image.get_layout(VkImageLayout.TransferDstOptimal), num_blits, blits);
        }

        internal void copy_image_to_buffer(Buffer buffer, Image image, uint num_blits, VkBufferImageCopy* blits)
        {

            vkCmdCopyImageToBuffer(handle, image.handle, image.get_layout(VkImageLayout.TransferSrcOptimal), buffer.handle, num_blits, blits);
        }



        internal void copy_buffer_to_image(Image image, Buffer src, ulong buffer_offset, VkOffset3D offset, VkExtent3D extent, uint row_length, uint slice_height, VkImageSubresourceLayers subresource)
        {
            VkBufferImageCopy region = new()
            {
                bufferOffset = buffer_offset,
                bufferRowLength = row_length,
                bufferImageHeight = slice_height,
                imageSubresource = subresource,
                imageOffset = offset,
                imageExtent = extent,
            };

            vkCmdCopyBufferToImage(handle, src.handle, image.handle, image.get_layout(VkImageLayout.TransferDstOptimal), 1, &region);
        }

        internal void copy_buffer_to_image(Buffer src, Image image, ulong buffer_offset, VkOffset3D offset, VkExtent3D extent, uint row_length, uint slice_height, VkImageSubresourceLayers subresource)
        {
            VkBufferImageCopy region = new()
            {
                bufferOffset = buffer_offset,
                bufferRowLength = row_length,
                bufferImageHeight = slice_height,
                imageSubresource = subresource,
                imageOffset = offset,
                imageExtent = extent,
            };

            vkCmdCopyBufferToImage(handle, src.handle, image.handle, image.get_layout(VkImageLayout.TransferSrcOptimal), 1, &region);
        }


        // TODO: ALL_GPUS  
        internal void set_current_gpu(int gpu_index)
        {
            //if (NativeDevice.device_count > 1)
            //{
            //    if (gpu_index == ALL_GPUS)
            //        vkCmdSetDeviceMaskKHR(handle, (1 << NativeDevice.device_count) - 1);
            //    else
            //        vkCmdSetDeviceMaskKHR(handle, 1 << gpu_index);
            //}
        }

        public void PipelineBarrier(VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* memoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* bufferMemoryBarriers, VkImageMemoryBarrier imageMemoryBarriers)
        {
            vkCmdPipelineBarrier(handle, srcStageMask, dstStageMask, dependencyFlags, memoryBarrierCount, memoryBarriers, bufferMemoryBarrierCount, null, 1, &imageMemoryBarriers);
        }

        public void PipelineBarrier(VkPipelineStageFlags srcStageMask, VkPipelineStageFlags dstStageMask, VkDependencyFlags dependencyFlags, uint memoryBarrierCount, VkMemoryBarrier* memoryBarriers, uint bufferMemoryBarrierCount, VkBufferMemoryBarrier* bufferMemoryBarriers, VkImageMemoryBarrier[] imageMemoryBarriers)
        {

        }


        public void SetCullMode(VkCullModeFlags mode)
        {
            vkCmdSetCullModeEXT(handle, mode);
        }

        public void SetLineWidth(float lineWidth)
        {
            vkCmdSetLineWidth(handle, lineWidth);
        }

        public void SetFrontFace(VkFrontFace frontFace)
        {
            vkCmdSetFrontFaceEXT(handle, frontFace);
        }

        public void SetPrimitiveTopology(VkPrimitiveTopology type)
        {
            vkCmdSetPrimitiveTopologyEXT(handle, type);
        }


        public void Dispatch(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
        {
            vkCmdDispatch(handle, (uint)threadGroupCountX, (uint)threadGroupCountY, (uint)threadGroupCountZ);

            VkMemoryBarrier barrier = new()
            {
                sType = VkStructureType.MemoryBarrier,
                pNext = null,
                dstAccessMask = VkAccessFlags.ShaderRead,
                srcAccessMask = VkAccessFlags.ShaderWrite
            };
            //vkCmdPipelineBarrier(handle, VkPipelineStageFlags.ComputeShader, VkPipelineStageFlags.AllGraphics, 0, 1, &barrier, 0, null, 0, null);
        }



        private void full_barrier()
        {

        }


        private void fixup_src_stage(ref VkPipelineStageFlags src_stages, bool fixup)
        {
            // ALL_GRAPHICS_BIT waits for vertex as well which causes performance issues on some drivers.
            // It shouldn't matter, but hey.
            //
            // We aren't using vertex with side-effects on relevant hardware so dropping VERTEX_SHADER_BIT is fine.
            if ((src_stages & VkPipelineStageFlags.AllGraphics) != 0 && fixup)
            {
                src_stages &= ~VkPipelineStageFlags.AllGraphics;
                src_stages |= VkPipelineStageFlags.ColorAttachmentOutput | VkPipelineStageFlags.FragmentShader | VkPipelineStageFlags.LateFragmentTests;
            }
        }



        private void barrier(VkPipelineStageFlags src_stages, VkAccessFlags src_access, VkPipelineStageFlags dst_stages,
                            VkAccessFlags dst_access)
        {
            VkMemoryBarrier barrier = new()
            {
                sType = VkStructureType.MemoryBarrier,
            };
            barrier.srcAccessMask = src_access;
            barrier.dstAccessMask = dst_access;
            vkCmdPipelineBarrier(handle, src_stages, dst_stages, 0, 1, &barrier, 0, null, 0, null);
        }



        public void Dispatch2D(int threadCountX, int threadCountY, int groupSizeX = 8, int groupSizeY = 8)
        {
            Dispatch(Helpers.ComputeWorkGroupCount(threadCountX, groupSizeX), Helpers.ComputeWorkGroupCount(threadCountY, groupSizeY), 1);
        }


        public void SetGraphicPipeline(GraphicsPipeline pipeline)
        {
            vkCmdBindPipeline(handle, VkPipelineBindPoint.Graphics, pipeline.graphicsPipeline);
        }


        public void SetComputePipeline(ComputePipeline pipeline)
        {
            vkCmdBindPipeline(handle, VkPipelineBindPoint.Compute, pipeline.handle);
        }

        public void SetRayTracingPipeline(GraphicsPipeline pipelineState)
        {
            //vkCmdBindPipeline(NativeCommandBuffer, VkPipelineBindPoint.RayTracingNV, pipelineState.rayTracinPipeline);
        }


        public void SetScissor(int width, int height, int x, int y)
        {
            // Update dynamic scissor state
            VkRect2D scissor = new(x, y, width, height);

            vkCmdSetScissor(handle, 0, 1, &scissor);
        }

        public void SetViewport(float Width, float Height, float X, float Y, float MinDepth = 0.0f, float MaxDepth = 1.0f)
        {
            float vpY = Height - Y;
            float vpHeight = -Height;


            VkViewport Viewport = new(X, Y, Width, Height, MinDepth, MaxDepth);
            vkCmdSetViewport(handle, 0, 1, &Viewport);
        }

        public void SetVertexBuffer(Buffer buffer, ulong offsets = 0)
        {
            fixed (VkBuffer* bufferptr = &buffer.handle)
            {
                vkCmdBindVertexBuffers(handle, 0, 1, bufferptr, &offsets);
            }
        }

        public void SetVertexBuffers(Buffer[] buffers, ulong offsets = 0)
        {
            VkBuffer* buffer = stackalloc VkBuffer[buffers.Length];

            for (int i = 0; i < buffers.Length; i++)
            {
                buffer[i] = buffers[i].handle;
            }

            //fixed(VkBuffer* bufferptr = &buffers[0].Handle)
            //{

            //}

            vkCmdBindVertexBuffers(handle, 0, 1, buffer, &offsets);
        }

        public void SetIndexBuffer(Buffer buffer, ulong offsets = 0, VkIndexType indexType = VkIndexType.Uint32)
        {
            if (buffer.handle != VkBuffer.Null)
            {
                vkCmdBindIndexBuffer(handle, buffer.handle, offsets, indexType);
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

        public void PushConstant<T>(GraphicsPipeline pipelineLayout, ShaderStage stageFlags, T data, uint offset = 0) where T : unmanaged
        {
            vkCmdPushConstants(handle, pipelineLayout._pipelineLayout, stageFlags.StageToVkShaderStageFlags(), offset, (uint)Interop.SizeOf<T>(), (void*)&data /*Interop.AllocToPointer<T>(ref data)*/);
        }



        public void Close()
        {
            if (set_render_pass)
                CleanupRenderPass();

            vkEndCommandBuffer(handle);
        }


        internal unsafe void CleanupRenderPass()
        {
            vkCmdEndRenderPass(handle);
        }


        public void BindDescriptorSets(DescriptorSet descriptor, uint dynamicOffsetCount = 0, uint dynamicOffsets = 0)
        {
            // Bind descriptor sets describing shader binding points
            VkDescriptorSet descriptor_set = descriptor._descriptorSet;
            VkPipelineLayout pipeline_layout = descriptor._pipelineLayout;

            for (int i = 0; i < descriptor.DescriptorData.Data.Count; i++)
            {
                ResourceData data = descriptor.DescriptorData.Data[i];
                if (data.DescriptorType == VkDescriptorType.StorageImage)
                {

                }
            }

            vkCmdBindDescriptorSets(handle, descriptor.BindPoint, pipeline_layout, 0, 1, &descriptor_set, dynamicOffsetCount, &dynamicOffsets);
        }




        public void BeginRenderPassContinue()
        {

            VkCommandBufferBeginInfo cmd_buffer_info = new VkCommandBufferBeginInfo()
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.RenderPassContinue,
                pNext = null,
            };

            vkBeginCommandBuffer(handle, &cmd_buffer_info);
        }

        public void BeginOneTimeSubmit()
        {

            VkCommandBufferBeginInfo cmd_buffer_info = new VkCommandBufferBeginInfo()
            {
                sType = VkStructureType.CommandBufferBeginInfo,
                flags = VkCommandBufferUsageFlags.OneTimeSubmit,
                pNext = null,
            };

            vkBeginCommandBuffer(handle, &cmd_buffer_info);
        }

        public void Free()
        {


            vkFreeCommandBuffers(NativeDevice.handle, cmd_command_pool, handle);
        }

        public void Dispose()
        {
            WaitFence.Dispose();
            vkDestroyCommandPool(NativeDevice.handle, cmd_command_pool, null);
        }


        public void End()
        {
            vkEndCommandBuffer(handle);
        }
    }
}
