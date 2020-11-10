// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture2D.cs
=============================================================================*/


using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe static class Texture2D 
    {

        //public Texture2D(Device device) : base(device, new TextureDescription() 
        //{ 
        //    Flags = TextureFlags.ShaderResource,

        //})
        //{
        //    //Recreate();
        //}


        public static unsafe Texture LoadTexture2D(Device ctx, TextureData tex2D)
        {
            ulong size = (ulong)tex2D.Size;
            VkBuffer stagingBuffer;
            var bufferCreateInfo = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                pNext = null,
                size = (ulong)tex2D.Size,
                usage = VkBufferUsageFlags.TransferSrc
            };
            vkCreateBuffer(ctx.handle, &bufferCreateInfo, null, out stagingBuffer);

            tex2D.MipMaps = 1; // TODO: MipMaps

            vkGetBufferMemoryRequirements(ctx.handle, stagingBuffer, out VkMemoryRequirements stagingMemReq);
            uint heapIndex = ctx.GetMemoryTypeIndex(stagingMemReq.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = stagingMemReq.size,
                memoryTypeIndex = heapIndex
            };

            VkDeviceMemory stagingMemory;
            VkResult result = vkAllocateMemory(ctx.handle, &memAllocInfo, null, &stagingMemory);
            result.CheckResult();

            result = vkBindBufferMemory(ctx.handle, stagingBuffer, stagingMemory, 0);
            result.CheckResult();

            void* vertexPtr;
            result = vkMapMemory(ctx.handle, stagingMemory, 0, (ulong)tex2D.Size, 0, &vertexPtr);
            result.CheckResult();

            fixed (byte* dataPtr = &tex2D.Data[0])
            {
                Interop.MemoryHelper.Write(new IntPtr(vertexPtr), tex2D.Data);


                vkUnmapMemory(ctx.handle, stagingMemory);

                // Setup buffer copy regions for each mip level.
                var bufferCopyRegions = new VkBufferImageCopy[tex2D.MipMaps]; // TODO: stackalloc
                int offset = 0;
                for (int i = 0; i < bufferCopyRegions.Length; i++)
                {

                    bufferCopyRegions = new[]
                    {
                    new VkBufferImageCopy
                    {
                        imageSubresource = new VkImageSubresourceLayers(VkImageAspectFlags.Color, (uint)i, 0, 1),
                        imageExtent = new VkExtent3D(tex2D.Width, tex2D.Height,1),
                        bufferOffset = (ulong)offset
                    }
                };
                    offset += tex2D.Size;
                }

                // Create optimal tiled target image.
                var createInfo = new VkImageCreateInfo
                {
                    sType = VkStructureType.ImageCreateInfo,
                    pNext = null,
                    imageType = VkImageType.Image2D,
                    format = (VkFormat)tex2D.Format,
                    mipLevels = (uint)tex2D.MipMaps,
                    arrayLayers = 1,
                    samples = VkSampleCountFlags.Count1,
                    tiling = VkImageTiling.Optimal,
                    sharingMode = VkSharingMode.Exclusive,
                    initialLayout = VkImageLayout.Undefined,
                    extent = new VkExtent3D(tex2D.Width, tex2D.Height, 1),
                    usage = VkImageUsageFlags.Sampled | VkImageUsageFlags.TransferDst
                };

                VkImage image;
                result = vkCreateImage(ctx.handle, &createInfo, null, out image);
                result.CheckResult();

                VkMemoryRequirements imageMemReq;
                vkGetImageMemoryRequirements(ctx.handle, image, out imageMemReq);

                vkGetPhysicalDeviceMemoryProperties(ctx.NativeAdapter.handle, out VkPhysicalDeviceMemoryProperties imageMemoryProperties);

                uint imageHeapIndex = ctx.GetMemoryTypeIndex(imageMemReq.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

                var allocInfo = new VkMemoryAllocateInfo
                {
                    sType = VkStructureType.MemoryAllocateInfo,
                    pNext = null,
                    allocationSize = imageMemReq.size,
                    memoryTypeIndex = imageHeapIndex,
                };
                VkDeviceMemory memory;
                result = vkAllocateMemory(ctx.handle, &allocInfo, null, &memory);
                result.CheckResult();

                result = vkBindImageMemory(ctx.handle, image, memory, 0);
                result.CheckResult();

                var subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, (uint)tex2D.MipMaps, 0, 1);

                // Copy the data from staging buffers to device local buffers.
                var allocInfo2 = new VkCommandBufferAllocateInfo()
                {
                    sType = VkStructureType.CommandBufferAllocateInfo,
                    commandPool = ctx.graphics_cmd_pool,

                    level = VkCommandBufferLevel.Primary,
                    commandBufferCount = 1,
                };
                VkCommandBuffer cmdBuffer;
                vkAllocateCommandBuffers(ctx.handle, &allocInfo2, &cmdBuffer);

                VkCommandBufferBeginInfo beginInfo = new VkCommandBufferBeginInfo()
                {
                    sType = VkStructureType.CommandBufferBeginInfo,
                    flags = VkCommandBufferUsageFlags.OneTimeSubmit,
                };

                vkBeginCommandBuffer(cmdBuffer, &beginInfo);

                VkImageMemoryBarrier imageMemoryBarrier = new VkImageMemoryBarrier
                {
                    sType = VkStructureType.ImageMemoryBarrier,
                    pNext = null,
                    image = image,
                    subresourceRange = subresourceRange,
                    srcAccessMask = 0,
                    dstAccessMask = VkAccessFlags.TransferWrite,
                    oldLayout = VkImageLayout.Undefined,
                    newLayout = VkImageLayout.TransferDstOptimal,
                    srcQueueFamilyIndex = QueueFamilyIgnored,
                    dstQueueFamilyIndex = QueueFamilyIgnored
                };

                vkCmdPipelineBarrier(cmdBuffer, VkPipelineStageFlags.TopOfPipe, VkPipelineStageFlags.Transfer, VkDependencyFlags.None, 0, null, 0, null, 1, &imageMemoryBarrier);

                fixed (VkBufferImageCopy* regionsPtr = bufferCopyRegions)
                {
                    vkCmdCopyBufferToImage(cmdBuffer, stagingBuffer, image, VkImageLayout.TransferDstOptimal, (uint)bufferCopyRegions.Length, regionsPtr);
                }

                VkImageMemoryBarrier imageMemoryBarrier2 = new VkImageMemoryBarrier
                {
                    sType = VkStructureType.ImageMemoryBarrier,
                    pNext = null,
                    image = image,
                    subresourceRange = subresourceRange,
                    srcAccessMask = VkAccessFlags.TransferWrite,
                    dstAccessMask = VkAccessFlags.ShaderRead,
                    oldLayout = VkImageLayout.TransferDstOptimal,
                    newLayout = VkImageLayout.ShaderReadOnlyOptimal,
                    srcQueueFamilyIndex = (uint)QueueFamilyIgnored,
                    dstQueueFamilyIndex = (uint)QueueFamilyIgnored
                };

                vkCmdPipelineBarrier(cmdBuffer, VkPipelineStageFlags.Transfer, VkPipelineStageFlags.FragmentShader, VkDependencyFlags.None, 0, null, 0, null, 1, &imageMemoryBarrier2);

                vkEndCommandBuffer(cmdBuffer);

                // Submit.
                VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo
                {
                    sType = VkStructureType.FenceCreateInfo,
                    pNext = null
                };
                VkFence fence;
                result = vkCreateFence(ctx.handle, &fenceCreateInfo, null, out fence);
                result.CheckResult();

                var submitInfo = new VkSubmitInfo
                {
                    sType = VkStructureType.SubmitInfo,
                    pNext = null,
                    commandBufferCount = 1,
                    pCommandBuffers = &cmdBuffer
                };

                vkQueueSubmit(ctx.nativeCommandQueue, submitInfo, fence);

                result = vkWaitForFences(ctx.handle, 1, &fence, false, ulong.MaxValue);
                result.CheckResult();

                // Cleanup staging resources.
                vkDestroyFence(ctx.handle, fence, null);
                vkFreeMemory(ctx.handle, stagingMemory, null);
                vkDestroyBuffer(ctx.handle, stagingBuffer, null);

                // Create image view.
                VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo()
                {
                    sType = VkStructureType.ImageViewCreateInfo,
                    image = image,
                    viewType = VkImageViewType.Image2D,
                    format = (VkFormat)tex2D.Format,
                    subresourceRange = subresourceRange
                };

                VkImageView view;
                vkCreateImageView(ctx.handle, &imageViewCreateInfo, null, out view);

                return new Texture(ctx, new TextureDescription
                {
                    Flags = TextureFlags.DepthStencil,
                    Usage = GraphicsResourceUsage.Default,
                    Width = 0,
                    Height = 0,
                })
                {
                    handle = image,
                    View = view,
                };
            }
        }
    }
}
