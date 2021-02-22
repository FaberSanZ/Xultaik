// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Texture.cs
=============================================================================*/


using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Vulkan
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

    public enum ImageViewMiscFlagBits
    {
        ForceArray = 1 << 0
    }


    public unsafe class Image : GraphicsResource, IDisposable
    {
        internal Buffer buffer;

        internal ulong row_pitch;
        internal ulong row_offset;

        internal VkImage handle;
        internal Layout layout_type = Layout.Optimal;
        internal VkPipelineStageFlags stage_flags = 0;
        internal VkAccessFlags access_flags = 0;
        internal VkImageLayout swapchain_layout = VkImageLayout.Undefined;
        internal bool owns_image = true;
        internal bool owns_memory_allocation = true;

        internal VkDeviceMemory memory;
        internal VkDeviceMemory buffer_memory;
        //internal VkBuffer buffer;
        internal uint layers;
        internal VkFormat format;

        internal VkImageView image_view;
        internal VkImageView depth_stencil_view;


        public Image(Device device, ImageDescription description) : base(device)
        {
            Description = description;

            Initialize();
        }



        public ImageDescription Description { get; set; }
        public ImageDimension Dimension => Description.Dimension;
        public PixelFormat Format => Description.Format;
        public byte[] Data => Description.Data;
        public int Size => Description.Size;
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
        public int Levels { get; internal set; }

        internal void Initialize()
        {
            //create_buffer_image(0);


            create_image();

            // For depth-stencil formats, automatically fall back to a supported one
            if (IsDepthStencil)
            {
                depth_stencil_view = get_depth_stencil_view();
                format = NativeDevice.NativeAdapter.get_supported_depth_format(PixelFormatExtensions.depth_formats);
            }

            if (IsShaderResource)
            {
                create_buffer_image();

                image_view = get_image_view();
            }



        }


        internal void create_buffer_image()
        {
            buffer = new Buffer(NativeDevice, new()
            {
                BufferFlags = BufferFlags.ShaderResource,
                SizeInBytes = Size,
                ByteStride = Size,
                Usage = GraphicsResourceUsage.Dynamic
            });


            buffer.SetData(Data);

        }


        internal void create_image()
        {

            // Create a new image
            VkImageCreateInfo image_create_info = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                arrayLayers = (uint)1, // TODO: arrayLayers
                extent = new(Width, Height, Depth),
                mipLevels = (uint)1,
                samples = VkSampleCountFlags.Count1,
                format = (VkFormat)Format,
                flags = VkImageCreateFlags.None,
                tiling = VkImageTiling.Optimal,
                initialLayout = VkImageLayout.Undefined
            };






            switch (Dimension)
            {
                case ImageDimension.Image1D:
                    image_create_info.imageType = VkImageType.Image1D;
                    break;
                case ImageDimension.Image2D:
                    image_create_info.imageType = VkImageType.Image2D;
                    break;
                case ImageDimension.Image3D:
                    image_create_info.imageType = VkImageType.Image3D;
                    break;
                case ImageDimension.ImageCube:
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
                image_create_info.format = NativeDevice.NativeAdapter.get_supported_depth_format(PixelFormatExtensions.depth_formats);
                image_create_info.mipLevels = 1;
                image_create_info.arrayLayers = 1;
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





            vkCreateImage(NativeDevice.handle, &image_create_info, null, out handle);


            vkGetImageMemoryRequirements(NativeDevice.handle, handle, out VkMemoryRequirements imageMemReq);

            VkMemoryPropertyFlags memoryProperties = VkMemoryPropertyFlags.DeviceLocal;

            uint imageHeapIndex = NativeDevice.GetMemoryTypeIndex(imageMemReq.memoryTypeBits, memoryProperties);

            VkMemoryAllocateInfo allocInfo = new VkMemoryAllocateInfo
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = imageMemReq.size,
                memoryTypeIndex = imageHeapIndex,
            };

            VkDeviceMemory _memory = default;
            vkAllocateMemory(NativeDevice.handle, &allocInfo, null, out _memory).CheckResult();
            memory = _memory;



            vkBindImageMemory(NativeDevice.handle, handle, memory, 0).CheckResult();
            

        }

        internal VkImageLayout get_layout(VkImageLayout imageLayout)
        {
            throw new NotImplementedException();
        }




        public void LoadTexture2D()
        {


            Description.MipLevels = 1; // TODO: MipMaps

            VkImageSubresourceRange subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, (uint)MipLevels, 0, 1);

            // Copy the data from staging buffers to device local buffers.
            VkCommandBufferAllocateInfo allocInfo2 = new VkCommandBufferAllocateInfo()
            {
                sType = VkStructureType.CommandBufferAllocateInfo,
                commandPool = NativeDevice.transfer_cmd_pool,

                level = VkCommandBufferLevel.Primary,
                commandBufferCount = 1,
            };
            VkCommandBuffer cmdBuffer;
            vkAllocateCommandBuffers(NativeDevice.handle, &allocInfo2, &cmdBuffer);

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
                image = handle,
                subresourceRange = subresourceRange,
                srcAccessMask = 0,
                dstAccessMask = VkAccessFlags.TransferWrite,
                oldLayout = VkImageLayout.Undefined,
                newLayout = VkImageLayout.TransferDstOptimal,
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            vkCmdPipelineBarrier(cmdBuffer, VkPipelineStageFlags.TopOfPipe, VkPipelineStageFlags.Transfer, VkDependencyFlags.None, 0, null, 0, null, 1, &imageMemoryBarrier);





            // Setup buffer copy regions for each mip level.
            VkBufferImageCopy[] bufferCopyRegions = new VkBufferImageCopy[MipLevels]; // TODO: stackalloc
            int offset = 0;
            for (int i = 0; i < bufferCopyRegions.Length; i++)
            {

                bufferCopyRegions = new[]
                {
                    new VkBufferImageCopy
                    {
                        imageSubresource = new VkImageSubresourceLayers(VkImageAspectFlags.Color, (uint)i, 0, 1),
                        imageExtent = new(Width, Height, 1),
                        bufferOffset = (ulong)offset
                    }
                };
                offset += Size;
            }

            fixed (VkBufferImageCopy* regionsPtr = bufferCopyRegions)
            {
                vkCmdCopyBufferToImage(cmdBuffer, buffer.handle, handle, VkImageLayout.TransferDstOptimal, (uint)bufferCopyRegions.Length, regionsPtr);
            }


            VkImageMemoryBarrier imageMemoryBarrier2 = new VkImageMemoryBarrier
            {
                sType = VkStructureType.ImageMemoryBarrier,
                pNext = null,
                image = handle,
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



            Fence fence = new Fence(NativeDevice);


            VkSubmitInfo submitInfo = new VkSubmitInfo
            {
                sType = VkStructureType.SubmitInfo,
                pNext = null,
                commandBufferCount = 1,
                pCommandBuffers = &cmdBuffer
            };

            vkQueueSubmit(NativeDevice.transfer_queue, submitInfo, fence.handle);

            fence.Wait();




            // Cleanup staging resources.
            fence.Dispose();
            vkFreeMemory(NativeDevice.handle, buffer_memory, null);
            vkDestroyBuffer(NativeDevice.handle, buffer.handle, null);






        }


        internal VkImageView get_depth_stencil_view()
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
                format = format,
                image = handle,
                components = VkComponentMapping.Identity,
                subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Depth | VkImageAspectFlags.Stencil, 0, 1, 0, 1)
            };


            vkCreateImageView(NativeDevice.handle, &createInfo, null, out VkImageView _view).CheckResult();
            return _view;
        }


        internal VkImageView get_image_view()
        {

            if (!IsShaderResource)
                return VkImageView.Null;

            // Create image view.
            VkImageViewCreateInfo imageViewCreateInfo = new VkImageViewCreateInfo()
            {
                sType = VkStructureType.ImageViewCreateInfo,
                image = handle,
                viewType = VkImageViewType.Image2D,
                format = (VkFormat)Format,
                subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, (uint)1, 0, 1), // TODO: MipMaps
            };

            vkCreateImageView(NativeDevice.handle, &imageViewCreateInfo, null, out var _view);

            return _view;
        }


        private void set_data()
        {
        }

        private void set_data_cubemap()
        {
        }






        public void Dispose()
        {
            if (IsDepthStencil)
            {
                vkDestroyImageView(NativeDevice.handle, depth_stencil_view, null);
            }
            else
            {
                vkDestroyImageView(NativeDevice.handle, image_view, null);
            }

            if (handle != VkImage.Null)
            {
                vkDestroyImage(NativeDevice.handle, handle, null);
            }

            if (memory != VkDeviceMemory.Null)
            {
                vkFreeMemory(NativeDevice.handle, memory, null);
            }
        }

    }
}
