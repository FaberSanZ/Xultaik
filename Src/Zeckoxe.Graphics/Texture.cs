// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture.cs
=============================================================================*/


using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Zeckoxe.Graphics
{
    [Flags]
    public enum TextureFlags
    {

        None = 0,

        ShaderResource = 1,


        RenderTarget = 2,


        UnorderedAccess = 4,


        DepthStencil = 8,
    }


    internal enum TextureMiscFlag
    {
        GenerateMips = 1 << 0,

        ForceArray = 1 << 1,

        MutableSrgb = 1 << 2,

        ConcurrentQueueGraphics = 1 << 3,

        ConcurrentQueueAsyncCompute = 1 << 4,

        ConcurrentQueueAsyncGraphics = 1 << 5,

        ConcurrentQueueAsyncTransfer = 1 << 6,

        VeryfyFormatFeatureSampledLinearFilter = 1 << 7,

        LinearImageIgnoreDeviceLocal = 1 << 8,

        ForceNoDedicated = 1 << 9
    };


    public enum Layout
    {
        Optimal,
        General
    };


    public enum ViewType
    {
        Full = 0,


        Single = 1,


        ArrayBand = 2,


        MipBand = 3,
    }


    // TODO: Vulkan Memory Allocator
    public unsafe class Texture : GraphicsResource, IDisposable
    {
        internal enum ImageViewMiscFlagBits
        {
            IMAGE_VIEW_MISC_FORCE_ARRAY_BIT = 1 << 0
        };




        internal VkPointerSize row_pitch;
        internal VkPointerSize row_offset;

        internal VkImage handle;
        internal Layout layout_type = Layout.Optimal;
        internal VkPipelineStageFlags stage_flags = 0;
        internal VkAccessFlags access_flags = 0;
        internal VkImageLayout swapchain_layout = VkImageLayout.Undefined;
        internal bool owns_image = true;
        internal bool owns_memory_allocation = true;

        internal VkDeviceMemory memory;
        internal VkImageView View;
        internal VkFormat vkformat;
        internal VkBuffer buffer;

        internal VkImageView depth_stencil_view => get_depth_stencilview();


        public Texture(Device device, TextureDescription description) : base(device)
        {
            Description = description;

            Initialize();
        }



        public TextureDescription Description { get; set; }
        public ImageDimension Dimension => Description.Dimension;
        public PixelFormat Format => Description.Format;
        public byte[] Data => Description.Data;
        public int MipLevels => Description.MipLevels;
        public int ArraySize => Description.ArraySize;
        public int Width => Description.Width;
        public int Height => Description.Height;
        public int Depth => Description.Depth;
        public bool IsCubeMap => Description.IsCubeMap;
        public PixelFormat Formatt => Description.Format;
        public TextureFlags ViewFlags => Description.Flags;

        public bool IsRenderTarget => (ViewFlags & TextureFlags.RenderTarget) != 0;

        public bool IsDepthStencil => (ViewFlags & TextureFlags.DepthStencil) != 0;

        public bool IsShaderResource => (ViewFlags & TextureFlags.ShaderResource) != 0;

        public bool IsUnorderedAccess => (ViewFlags & TextureFlags.UnorderedAccess) != 0;

        public bool IsMultisample => MultisampleCount > MultisampleCount.None;



        public MultisampleCount MultisampleCount { get; private set; }

        internal void Initialize()
        {
            //create_buffer_image(0);



            // For depth-stencil formats, automatically fall back to a supported one
            if (IsDepthStencil)
            {
                vkformat = NativeDevice.NativeAdapter.get_supported_depth_format(PixelFormatExtensions.depth_formats);
                Description.MipLevels = 1;
                Description.ArraySize = 1;
            }

            create_image();





            Description.format = vkformat;

            if (IsCubeMap)
            {
                set_data_cubemap();
            }
            else
            {
                set_data();
            }



        }


        internal void create_buffer_image(ulong size)
        {

            VkBufferCreateInfo bufferCreateInfo = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                pNext = null,
                size = size,
                usage = VkBufferUsageFlags.TransferSrc | VkBufferUsageFlags.TransferDst
            };
            vkCreateBuffer(NativeDevice.handle, &bufferCreateInfo, null, out buffer);

            vkGetBufferMemoryRequirements(NativeDevice.handle, buffer, out VkMemoryRequirements stagingMemReq);
            uint heapIndex = NativeDevice.GetMemoryTypeIndex(stagingMemReq.memoryTypeBits, VkMemoryPropertyFlags.HostVisible);

            VkMemoryAllocateInfo memAllocInfo = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = stagingMemReq.size,
                memoryTypeIndex = heapIndex
            };

            VkDeviceMemory stagingMemory = default;
            vkAllocateMemory(NativeDevice.handle, &memAllocInfo, null, &stagingMemory).CheckResult();
            memory = stagingMemory;

            vkBindBufferMemory(NativeDevice.handle, buffer, memory, 0).CheckResult();

        }


        internal void create_image()
        {
            // Create a new image
            VkImageCreateInfo image_create_info = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                arrayLayers = (uint)ArraySize,
                extent = new VkExtent3D(Width, Height, Depth),
                mipLevels = (uint)MipLevels,
                samples = VkSampleCountFlags.Count1,
                format = vkformat,
                flags = VkImageCreateFlags.None,
                tiling = VkImageTiling.Optimal,
                initialLayout = VkImageLayout.Undefined
            };



            switch (Dimension)
            {
                case ImageDimension.Texture1D:
                    image_create_info.imageType = VkImageType.Image1D;
                    break;
                case ImageDimension.Texture2D:
                    image_create_info.imageType = VkImageType.Image2D;
                    break;
                case ImageDimension.Texture3D:
                    image_create_info.imageType = VkImageType.Image3D;
                    break;
                case ImageDimension.TextureCube:
                    image_create_info.imageType = VkImageType.Image2D;
                    image_create_info.flags |= VkImageCreateFlags.CubeCompatible;
                    break;
            }



            // TODO VULKAN: Can we restrict more based on GraphicsResourceUsage? 
            image_create_info.usage |= VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst;

            if (IsRenderTarget)
            {
                image_create_info.usage |= VkImageUsageFlags.ColorAttachment;
            }

            if (IsDepthStencil)
            {
                image_create_info.usage |= VkImageUsageFlags.DepthStencilAttachment;
            }

            if (IsShaderResource)
            {
                image_create_info.usage |= VkImageUsageFlags.Sampled; // TODO VULKAN: Input attachments
            }

            if (IsUnorderedAccess)
            {
                image_create_info.usage |= VkImageUsageFlags.Storage;
            }



            VkMemoryPropertyFlags memoryProperties = VkMemoryPropertyFlags.DeviceLocal;


            vkCreateImage(NativeDevice.handle, &image_create_info, null, out handle);


            vkGetImageMemoryRequirements(NativeDevice.handle, handle, out VkMemoryRequirements imageMemReq);


            uint imageHeapIndex = NativeDevice.GetMemoryTypeIndex(imageMemReq.memoryTypeBits, memoryProperties);

            VkMemoryAllocateInfo allocInfo = new VkMemoryAllocateInfo
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = imageMemReq.size,
                memoryTypeIndex = imageHeapIndex,
            };

            VkDeviceMemory _memory = default;
            vkAllocateMemory(NativeDevice.handle, &allocInfo, null, &_memory).CheckResult();
            memory = _memory;


            if (_memory != VkDeviceMemory.Null)
            {
                vkBindImageMemory(NativeDevice.handle, handle, memory, 0).CheckResult();
            }

        }




        internal VkImageView get_depth_stencilview()
        {
            if (!IsDepthStencil)
            {
                return VkImageView.Null;
            }

            // Create a Depth stencil view on this texture2D
            VkImageViewCreateInfo createInfo = new VkImageViewCreateInfo
            {
                sType = VkStructureType.ImageViewCreateInfo,
                viewType = VkImageViewType.Image2D,
                format = vkformat,
                image = handle,
                components = VkComponentMapping.Identity,
                subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil, 0, 1, 0, 1)
            };


            vkCreateImageView(NativeDevice.handle, &createInfo, null, out VkImageView imageView).CheckResult();
            return imageView;
        }


        private void set_data()
        {
        }

        private void set_data_cubemap()
        {
        }




        public void Imple(ITextureData tex2D)
        {


            vkGetPhysicalDeviceMemoryProperties(NativeDevice.NativeAdapter.handle, out VkPhysicalDeviceMemoryProperties memoryProperties);
            vkGetBufferMemoryRequirements(NativeDevice.handle, buffer, out VkMemoryRequirements stagingMemReq);

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

            result = vkBindBufferMemory(NativeDevice.handle, buffer, stagingMemory, 0);
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
                vkCmdCopyBufferToImage(cmdBuffer, buffer, image, VkImageLayout.TransferDstOptimal, (uint)bufferCopyRegions.Length, regionsPtr);
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
            vkDestroyBuffer(NativeDevice.handle, buffer, null);

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

        public void Dispose()
        {
            if (IsDepthStencil)
                vkDestroyImageView(NativeDevice.handle, depth_stencil_view, null);
            else
                vkDestroyImageView(NativeDevice.handle, View, null);


            if (handle != VkImage.Null)
                vkDestroyImage(NativeDevice.handle, handle, null);


            if (memory != VkDeviceMemory.Null)
                vkFreeMemory(NativeDevice.handle, memory, null);

        }

    }
}
