// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	Image.cs
=============================================================================*/


using System;
using Vortice.Vulkan;
using Zeckoxe.Core;
using static Vortice.Vulkan.Vulkan;


namespace Zeckoxe.Graphics.Vulkan
{
    public unsafe class Image : GraphicsResource
    {
        internal struct ImageInitialData
        {
            internal void* data;
            internal VkPointerSize row_length;
            internal VkPointerSize image_height;
        };


        internal enum ImageMiscFlagBits
        {
            GenerateMipsBit = 1 << 0,

            FORCE_ARRAY_BIT = 1 << 1,
            MUTABLE_SRGB_BIT = 1 << 2,
            CONCURRENT_QUEUE_GRAPHICS_BIT = 1 << 3,
            CONCURRENT_QUEUE_ASYNC_COMPUTE_BIT = 1 << 4,
            CONCURRENT_QUEUE_ASYNC_GRAPHICS_BIT = 1 << 5,
            CONCURRENT_QUEUE_ASYNC_TRANSFER_BIT = 1 << 6,
            VERIFY_FORMAT_FEATURE_SAMPLED_LINEAR_FILTER_BIT = 1 << 7,
            LINEAR_IMAGE_IGNORE_DEVICE_LOCAL_BIT = 1 << 8,
            FORCE_NO_DEDICATED_BIT = 1 << 9
        };

        internal enum ImageViewMiscFlagBits
        {
            IMAGE_VIEW_MISC_FORCE_ARRAY_BIT = 1 << 0
        };

        internal enum Layout
        {
            Optimal,
            General
        };


        internal VkPointerSize row_pitch;
        internal VkPointerSize row_offset;

        internal GraphicsDevice device;
        internal VkImage image;
        internal Layout layout_type = Layout.Optimal;
        internal VkPipelineStageFlags stage_flags = 0;
        internal VkAccessFlags access_flags = 0;
        internal VkImageLayout swapchain_layout = VkImageLayout.Undefined;
        internal bool owns_image = true;
        internal bool owns_memory_allocation = true;

        internal VkDeviceMemory Mem;
        internal VkImageView View;
        internal VkFormat vkformat;
        internal VkBuffer buffer;




        public Image(GraphicsDevice device) : base(device)
        {
            device = NativeDevice;
        }



        public TextureDescription Description { get; set; }
        public TextureDimension Dimension => Description.Dimension;
        public PixelFormat ViewFormat => Description.Format;
        public int MipLevels => Description.MipLevels;
        public int ArraySize => Description.ArraySize;
        public int Width => Description.Width;
        public int Height => Description.Height;
        public int Depth => Description.Depth;
        public PixelFormat Formatt => Description.Format;



        private unsafe void CreateBuffer(ulong size)
        {
            VkBufferCreateInfo createInfo = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                flags = VkBufferCreateFlags.None
            };

            createInfo.size = size;

            createInfo.usage = VkBufferUsageFlags.TransferSrc | VkBufferUsageFlags.TransferDst;

            // Create buffer
            vkCreateBuffer(device.handle, &createInfo, null, out buffer);

            // Allocate and bind memory
            vkGetBufferMemoryRequirements(device.handle, buffer, out VkMemoryRequirements memoryRequirements);

            //VkDeviceMemory stagingMemory;
            //VkResult result = vkAllocateMemory(NativeDevice.handle, &memAllocInfo, null, &stagingMemory);
            //result.CheckResult();


            if (Mem != VkDeviceMemory.Null)
            {
                vkBindBufferMemory(device.handle, buffer, Mem, 0);
            }
        }

        public void CreateDepthStencil(int width, int height)
        {
            VkFormat format = (VkFormat)NativeDevice.NativeAdapter.GetSupportedDepthFormat(PixelFormatExtensions.DepthFormats); // TODO: ToVkFormat



            VkImageCreateInfo imageCreateInfo = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                pNext = null,
                imageType = VkImageType.Image2D,
                format = format,
                extent = new Vortice.Mathematics.Size3
                {
                    Width = width,
                    Height = height,
                    Depth = 1
                },
                mipLevels = 1,
                arrayLayers = 1,
                samples = VkSampleCountFlags.Count1,
                tiling = VkImageTiling.Optimal,
                usage = VkImageUsageFlags.DepthStencilAttachment | VkImageUsageFlags.TransferSrc
            };

            VkResult result = vkCreateImage(NativeDevice.handle, &imageCreateInfo, null, out image);
            result.CheckResult();

            vkGetImageMemoryRequirements(NativeDevice.handle, image, out VkMemoryRequirements memReq);


            uint heapIndex = NativeDevice.GetMemoryTypeIndex(memReq.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = memReq.size,
                memoryTypeIndex = heapIndex
            };

            VkDeviceMemory memory;
            vkAllocateMemory(NativeDevice.handle, &memAllocInfo, null, &memory);

            vkBindImageMemory(NativeDevice.handle, image, memory, 0);

            VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo
            {
                sType = VkStructureType.ImageViewCreateInfo,
                pNext = null,
                format = format,
                subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil, 0, 1, 0, 1),
                image = image,
                viewType = VkImageViewType.Image2D
            };

            vkCreateImageView(NativeDevice.handle, &imageViewCreateInfo, null, out VkImageView view).CheckResult();


            Mem = memory;
            View = view;
            vkformat = format;
        }


        public void Imple(TextureData tex2D)
        {
            IntPtr pixelsPtr = IntPtr.Zero;

            //Interop.MemoryHelper.Copy(pixelsPtr, dataBox.DataPointer, dataBox.SlicePitch);


            ulong size = (ulong)tex2D.MipMaps;
            VkBufferCreateInfo bufferCreateInfo = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                pNext = null,
                size = (ulong)tex2D.MipMaps,
                usage = VkBufferUsageFlags.TransferSrc
            };
            vkCreateBuffer(NativeDevice.handle, &bufferCreateInfo, null, out VkBuffer stagingBuffer);

            vkGetPhysicalDeviceMemoryProperties(NativeDevice.NativeAdapter.handle, out VkPhysicalDeviceMemoryProperties memoryProperties);
            vkGetBufferMemoryRequirements(NativeDevice.handle, stagingBuffer, out VkMemoryRequirements stagingMemReq);
            uint heapIndex = NativeDevice.GetMemoryTypeIndex(stagingMemReq.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = stagingMemReq.size,
                memoryTypeIndex = heapIndex
            };

            VkDeviceMemory stagingMemory;
            VkResult result = vkAllocateMemory(NativeDevice.handle, &memAllocInfo, null, &stagingMemory);
            result.CheckResult();

            result = vkBindBufferMemory(NativeDevice.handle, stagingBuffer, stagingMemory, 0);
            result.CheckResult();

            void* vertexPtr;
            result = vkMapMemory(NativeDevice.handle, stagingMemory, 0, 1, 0, &vertexPtr); // TODO: Mipmaps
            result.CheckResult();

            // TODO: MemoryCopy Texture
            //fixed (byte* dataPtr = &tex2D.Data)
            //{

            //    Interop.MemoryHelper.Copy(pixelsPtr, dataBox.DataPointer, dataBox.SlicePitch);

            //}

            vkUnmapMemory(NativeDevice.handle, stagingMemory);

            // Setup buffer copy regions for each mip level.
            VkBufferImageCopy[] bufferCopyRegions = new VkBufferImageCopy[tex2D.MipMaps]; // TODO: stackalloc and Mipmaps
            int offset = 0;
            for (int i = 0; i < bufferCopyRegions.Length; i++)
            {
                bufferCopyRegions = new[]
                {
                    new VkBufferImageCopy
                    {
                        imageSubresource = new VkImageSubresourceLayers(VkImageAspectFlags.Color, (uint)i, 0, 1),
                        // TODO: Mipmaps
                        //imageExtent = tex2D.Mipmaps[0].Extent, 
                        bufferOffset = (ulong)offset
                    }
                };
                offset += 1 >> i; // TODO: Mipmaps
            }

            // Create optimal tiled target image.
            VkImageCreateInfo createInfo = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                pNext = null,
                imageType = VkImageType.Image2D,
                format = (VkFormat)tex2D.Format, // TODO: ToVkFormat 
                mipLevels = 1, // TODO: Mipmaps
                arrayLayers = 1,
                samples = VkSampleCountFlags.Count1,
                tiling = VkImageTiling.Optimal,
                sharingMode = VkSharingMode.Exclusive,
                initialLayout = VkImageLayout.Undefined,
                //extent = tex2D.Mipmaps[0].Extent,
                usage = VkImageUsageFlags.Sampled | VkImageUsageFlags.TransferDst
            };

            vkCreateImage(NativeDevice.handle, &createInfo, null, out VkImage image);


            vkGetImageMemoryRequirements(NativeDevice.handle, image, out VkMemoryRequirements imageMemReq);


            uint imageHeapIndex = NativeDevice.GetMemoryTypeIndex(imageMemReq.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);

            VkMemoryAllocateInfo allocInfo = new VkMemoryAllocateInfo
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = imageMemReq.size,
                memoryTypeIndex = imageHeapIndex,
            };
            VkDeviceMemory memory;
            result = vkAllocateMemory(NativeDevice.handle, &allocInfo, null, &memory);
            result.CheckResult();

            result = vkBindImageMemory(NativeDevice.handle, image, memory, 0);
            result.CheckResult();

            VkImageSubresourceRange subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1); // TODO: Mipmaps

            // Copy the data from staging buffers to device local buffers.
            VkCommandBufferAllocateInfo allocInfo2 = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                //commandPool = ctx.GraphicsCommandPool,  
                //TODO: GraphicsCommandPool?

                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };
            VkCommandBuffer cmdBuffer = default;

            vkAllocateCommandBuffers(NativeDevice.handle, &allocInfo2, &cmdBuffer); //TODO: CommandBuffer.Copy() 

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
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            vkCmdPipelineBarrier(cmdBuffer, VkPipelineStageFlags.Transfer, VkPipelineStageFlags.FragmentShader, VkDependencyFlags.None, 0, null, 0, null, 1, &imageMemoryBarrier2);

            vkEndCommandBuffer(cmdBuffer);

            // Submit.
            VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo
            {
                sType = VkStructureType.FenceCreateInfo,
                pNext = null
            };
            result = vkCreateFence(NativeDevice.handle, &fenceCreateInfo, null, out VkFence fence);
            result.CheckResult();

            VkSubmitInfo submitInfo = new VkSubmitInfo
            {
                sType = VkStructureType.SubmitInfo,
                pNext = null,
                commandBufferCount = 1,
                pCommandBuffers = &cmdBuffer
            };

            vkQueueSubmit(NativeDevice.nativeCommandQueue, submitInfo, fence); // TODO: GraphicsQueue

            result = vkWaitForFences(NativeDevice.handle, 1, &fence, false, ulong.MaxValue);
            result.CheckResult();

            // Cleanup staging resources.
            vkDestroyFence(NativeDevice.handle, fence, null);
            vkFreeMemory(NativeDevice.handle, stagingMemory, null);
            vkDestroyBuffer(NativeDevice.handle, stagingBuffer, null);

            // Create image view.
            VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo()
            {
                sType = VkStructureType.ImageViewCreateInfo,
                image = image,
                viewType = VkImageViewType.Image2D,
                format = (VkFormat)tex2D.Format, // TODO: ToVkFormat 
                subresourceRange = subresourceRange
            };

            vkCreateImageView(NativeDevice.handle, &imageViewCreateInfo, null, out VkImageView view);

        }

    }
}
