// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Vultaik.Interop;

namespace Vultaik
{
    [Flags]
    public enum ImageFlags
    {

        None = 0,

        ShaderResource = 1,


        RenderTarget = 2,


        UnorderedAccess = 4,


        DepthStencil = 8,


        ReadWriteImage = 16,
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

        internal VkImageView image_view;


        public Image(Device device, ImageDescription description) : base(device)
        {
            Description = description;

            Initialize();
        }



        public ImageDescription Description { get; set; }
        public VkImageType ImageType => Description.ImageType;
        public VkFormat Format => Description.Format;
        public byte[]? Data => Description.Data;
        public int Size => Description.Size;
        public int MipLevels => Description.MipLevels;
        public int ArraySize => Description.ArraySize;
        public int Width => Description.Width;
        public int Height => Description.Height;
        public int Depth => Description.Depth;
        public bool IsCubeMap => Description.IsCubeMap;
        public ImageFlags ViewFlags => Description.Flags;

        public bool IsRenderTarget => (ViewFlags & ImageFlags.RenderTarget) != 0;

        public bool IsDepthStencil => (ViewFlags & ImageFlags.DepthStencil) != 0;

        public bool IsShaderResource => (ViewFlags & ImageFlags.ShaderResource) != 0;

        public bool IsUnorderedAccess => (ViewFlags & ImageFlags.UnorderedAccess) != 0;
        public bool IsReadWriteImage => (ViewFlags & ImageFlags.ReadWriteImage) != 0;

        public bool IsMultisample => MultisampleCount > VkSampleCountFlags.None;
        


        public VkSampleCountFlags MultisampleCount { get; private set; }

        internal void Initialize()
        {
            //create_buffer_image(0);


            create_image();

            // For depth-stencil formats, automatically fall back to a supported one
            if (IsDepthStencil)
            {
                image_view = get_depth_stencil_view();
            }

            if (IsShaderResource)
            {
                create_buffer_image();

                image_view = get_image_view();
            }



        }


        internal void Initialize(int w, int h)
        {
            //create_buffer_image(0);


            create_image(w, h);

            // For depth-stencil formats, automatically fall back to a supported one
            if (IsDepthStencil)
            {
                image_view = get_depth_stencil_view();
            }

            if (IsShaderResource)
            {
                create_buffer_image();

                image_view = get_image_view();
            }



        }

        internal void create_buffer_image()
        {


        }


        internal void create_image(int w, int h)
        {

            // Create a new image
            VkImageCreateInfo image_create_info = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                arrayLayers = 1, // TODO: arrayLayers
                extent = new(w, h, Depth),
                mipLevels = 1,
                samples = VkSampleCountFlags.Count1,
                format = Format,
                flags = VkImageCreateFlags.None,
                tiling = VkImageTiling.Optimal,
                initialLayout = VkImageLayout.Undefined,

                imageType = ImageType,
            };




            // TODO: Can we restrict more based on ResourceUsage? 
            image_create_info.usage |= VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst;

            if (IsCubeMap)
                image_create_info.flags |= VkImageCreateFlags.CubeCompatible;

            if (IsRenderTarget)
                image_create_info.usage |= VkImageUsageFlags.ColorAttachment;

            if (IsDepthStencil)
                image_create_info.usage |= VkImageUsageFlags.DepthStencilAttachment;

            if (IsShaderResource)
                image_create_info.usage |= VkImageUsageFlags.Sampled; // TODO: Input attachments

            if (IsUnorderedAccess)
                image_create_info.usage |= VkImageUsageFlags.Storage;


            if (IsReadWriteImage)
                image_create_info.usage |= VkImageUsageFlags.Storage;



            vkCreateImage(NativeDevice.handle, &image_create_info, null, out handle);


            vkGetImageMemoryRequirements(NativeDevice.handle, handle, out VkMemoryRequirements imageMemReq);

            VkMemoryPropertyFlags memoryProperties = VkMemoryPropertyFlags.DeviceLocal;

            uint imageHeapIndex = NativeDevice.get_memory_type(imageMemReq.memoryTypeBits, memoryProperties);
            Console.WriteLine(imageMemReq.size);
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
        internal void create_image()
        {

            // Create a new image
            VkImageCreateInfo image_create_info = new VkImageCreateInfo
            {
                sType = VkStructureType.ImageCreateInfo,
                arrayLayers = 1, // TODO: arrayLayers
                extent = new(Width, Height, Depth),
                mipLevels = 1,
                samples = VkSampleCountFlags.Count1,
                format = Format,
                flags = VkImageCreateFlags.None,
                tiling = VkImageTiling.Optimal,
                initialLayout = VkImageLayout.Undefined,

                imageType = ImageType,
            };




            // TODO: Can we restrict more based on GraphicsResourceUsage? 
            image_create_info.usage |= VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst;

            if (IsCubeMap)
                image_create_info.flags |= VkImageCreateFlags.CubeCompatible;

            if (IsRenderTarget)
                image_create_info.usage |= VkImageUsageFlags.ColorAttachment;

            if (IsDepthStencil)
                image_create_info.usage |= VkImageUsageFlags.DepthStencilAttachment;

            if (IsShaderResource)
                image_create_info.usage |= VkImageUsageFlags.Sampled; // TODO: Input attachments

            if (IsUnorderedAccess)
                image_create_info.usage |= VkImageUsageFlags.Storage;





            vkCreateImage(NativeDevice.handle, &image_create_info, null, out handle);


            vkGetImageMemoryRequirements(NativeDevice.handle, handle, out VkMemoryRequirements imageMemReq);

            VkMemoryPropertyFlags memoryProperties = VkMemoryPropertyFlags.DeviceLocal;

            uint imageHeapIndex = NativeDevice.get_memory_type(imageMemReq.memoryTypeBits, memoryProperties);

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
            return VkImageLayout.TransferDstOptimal;
        }


        public void SetData(byte[] data)
        {

            buffer = new Buffer(NativeDevice, new()
            {
                BufferFlags = BufferFlags.ShaderResource,
                SizeInBytes = Size,
                ByteStride = Size,
                Usage = ResourceUsage.CPU_To_GPU
            });


            buffer.SetData(data);

            CommandBuffer cmd = new(NativeDevice, CommandBufferType.AsyncTransfer);
            cmd.BeginOneTimeSubmit();


            VkImageSubresourceRange subresource_range = new(VkImageAspectFlags.Color, 0, (uint)MipLevels, 0, 1);

            VkImageMemoryBarrier memory_barrier = new()
            {
                sType = VkStructureType.ImageMemoryBarrier,
                pNext = null,
                image = handle,
                subresourceRange = subresource_range,
                srcAccessMask = VkAccessFlags.None,
                dstAccessMask = VkAccessFlags.TransferWrite,
                oldLayout = VkImageLayout.Undefined,
                newLayout = VkImageLayout.TransferDstOptimal,
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            cmd.PipelineBarrier(VkPipelineStageFlags.TopOfPipe, VkPipelineStageFlags.Transfer, VkDependencyFlags.None, 0, null, 0, null, memory_barrier);



            uint num_blits = (uint)1;
            int offset = 0;

            VkBufferImageCopy* blits = stackalloc VkBufferImageCopy[MipLevels]; // Setup buffer copy regions for each mip level.

            for (uint i = 0; i < num_blits; i++)
            {
                blits[i] = new()
                {
                    imageSubresource = new(VkImageAspectFlags.Color, i, 0, 1),
                    imageExtent = new(Width, Height, 1),
                    bufferOffset = (ulong)offset
                };

                offset += Size;
            }
            cmd.copy_buffer_to_image(handle, buffer.handle, num_blits, blits, VkImageLayout.TransferDstOptimal);



            VkImageMemoryBarrier memory_barrier_read = new()
            {
                sType = VkStructureType.ImageMemoryBarrier,
                pNext = null,
                image = handle,
                subresourceRange = subresource_range,
                srcAccessMask = VkAccessFlags.TransferWrite,
                dstAccessMask = VkAccessFlags.ShaderRead,
                oldLayout = VkImageLayout.TransferDstOptimal,
                newLayout = VkImageLayout.ShaderReadOnlyOptimal,
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            cmd.PipelineBarrier(VkPipelineStageFlags.Transfer, VkPipelineStageFlags.FragmentShader, VkDependencyFlags.None, 0, null, 0, null, memory_barrier_read);

            cmd.End();


            Fence fence = new Fence(NativeDevice);

            NativeDevice.Submit(cmd, fence);

            fence.Wait();

            //if (fence.IsSignaled)


            // Cleanup staging resources.
            fence.Dispose();
            vkFreeMemory(NativeDevice.handle, buffer_memory, null);
            vkDestroyBuffer(NativeDevice.handle, buffer.handle, null);

        }

        public void Image2D()
        {

            buffer = new Buffer(NativeDevice, new()
            {
                BufferFlags = BufferFlags.ShaderResource,
                SizeInBytes = Size,
                ByteStride = Size,
                Usage = ResourceUsage.CPU_To_GPU
            });


            buffer.SetData(Data);

            CommandBuffer cmd = new(NativeDevice, CommandBufferType.AsyncTransfer);
            cmd.BeginOneTimeSubmit();


            VkImageSubresourceRange subresource_range = new(VkImageAspectFlags.Color, 0, (uint)MipLevels, 0, 1);

            VkImageMemoryBarrier memory_barrier = new()
            {
                sType = VkStructureType.ImageMemoryBarrier,
                pNext = null,
                image = handle,
                subresourceRange = subresource_range,
                srcAccessMask =  VkAccessFlags.None,
                dstAccessMask = VkAccessFlags.TransferWrite,
                oldLayout = VkImageLayout.Undefined,
                newLayout = VkImageLayout.TransferDstOptimal,
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            cmd.PipelineBarrier(VkPipelineStageFlags.TopOfPipe, VkPipelineStageFlags.Transfer, VkDependencyFlags.None, 0, null, 0, null, memory_barrier);



            uint num_blits = (uint)1;
            int offset = 0;
            
            VkBufferImageCopy* blits = stackalloc VkBufferImageCopy[MipLevels]; // Setup buffer copy regions for each mip level.

            for (uint i = 0; i < num_blits; i++)
            {
                blits[i] = new()
                {
                    imageSubresource = new(VkImageAspectFlags.Color, i, 0, 1),
                    imageExtent = new(Width, Height, 1),
                    bufferOffset = (ulong)offset
                };

                offset += Size;
            }
            cmd.copy_buffer_to_image(handle, buffer.handle, num_blits, blits, VkImageLayout.TransferDstOptimal);



            VkImageMemoryBarrier memory_barrier_read = new()
            {
                sType = VkStructureType.ImageMemoryBarrier,
                pNext = null,
                image = handle,
                subresourceRange = subresource_range,
                srcAccessMask = VkAccessFlags.TransferWrite,
                dstAccessMask = VkAccessFlags.ShaderRead,
                oldLayout = VkImageLayout.TransferDstOptimal,
                newLayout = VkImageLayout.ShaderReadOnlyOptimal,
                srcQueueFamilyIndex = QueueFamilyIgnored,
                dstQueueFamilyIndex = QueueFamilyIgnored
            };

            cmd.PipelineBarrier(VkPipelineStageFlags.Transfer, VkPipelineStageFlags.FragmentShader, VkDependencyFlags.None, 0, null, 0, null, memory_barrier_read);

            cmd.End();


            Fence fence = new Fence(NativeDevice);

            NativeDevice.Submit(cmd, fence);

            fence.Wait();

            //if (fence.IsSignaled)


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
                flags = VkImageViewCreateFlags.None,
                pNext = null,
                viewType = VkImageViewType.Image2D,
                format = Format,
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
                format = Format,
                subresourceRange = new VkImageSubresourceRange(VkImageAspectFlags.Color, 0, 1, 0, 1), // TODO: MipMaps
            };

            vkCreateImageView(NativeDevice.handle, &imageViewCreateInfo, null, out var _view);

            return _view;
        }






        private void set_data()
        {
        }

        private void set_data_cube_map()
        {
        }




        public void Dispose()
        {

            vkDestroyImageView(NativeDevice.handle, image_view, null);
            

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
