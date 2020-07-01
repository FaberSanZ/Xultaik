// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Buffer.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Core;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    public unsafe class Buffer : GraphicsResource
    {
        public BufferDescription BufferDescription { get; set; }


        public int SizeInBytes
        {
            get => BufferDescription.SizeInBytes;
        }

        public int ByteStride
        {
            get => BufferDescription.ByteStride;
        }

        public GraphicsResourceUsage Usage
        {
            get => BufferDescription.Usage;
        }

        public BufferFlags Flags
        {
            get => BufferDescription.BufferFlags;
        }






        // internal
        internal VkBuffer Handle;
        internal VkBufferView NativeBufferView;
        internal VkAccessFlags NativeAccessMask;
        internal VkDeviceMemory memory;

        public Buffer(GraphicsDevice graphicsDevice, BufferDescription description) : base(graphicsDevice)
        {
            BufferDescription = description;

            Recreate();
        }

        private void Recreate()
        {
            VkBufferCreateInfo buffer_info = new VkBufferCreateInfo()
            {
                sType = VkStructureType.BufferCreateInfo,
                pNext = null,
                size = (ulong)BufferDescription.SizeInBytes,
                flags = VkBufferCreateFlags.None,
                //sharingMode = VkSharingMode.Exclusive
            };


            buffer_info.usage |= VkBufferUsageFlags.TransferSrc;


            if (Usage == GraphicsResourceUsage.Staging)
                NativeAccessMask = VkAccessFlags.HostRead | VkAccessFlags.HostWrite;
            
            else
            {
                if ((Flags/*.HasFlag()*/& BufferFlags.VertexBuffer) != 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.VertexBuffer;
                    NativeAccessMask |= VkAccessFlags.VertexAttributeRead;
                }

                if ((Flags & BufferFlags.IndexBuffer) != 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.IndexBuffer;
                    NativeAccessMask |= VkAccessFlags.IndexRead;
                }

                if ((Flags & BufferFlags.ConstantBuffer) != 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.UniformBuffer;
                    NativeAccessMask |= VkAccessFlags.UniformRead;
                }

                if ((Flags & BufferFlags.ShaderResource) != 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.UniformTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderRead;
                }

                if ((Flags & BufferFlags.UnorderedAccess) != 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.StorageTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderWrite;
                }
            }


            uint vertexBufferSize = (uint)SizeInBytes;
            VkDeviceMemory _memory;
            VkBuffer _buffer;



            // Copy vertex data to a buffer visible to the host
            vkCreateBuffer(NativeDevice.handle, &buffer_info, null, out _buffer);
            Handle = _buffer;
            VkMemoryRequirements memReqs;

            vkGetBufferMemoryRequirements(NativeDevice.handle, Handle, out memReqs);

            VkMemoryAllocateInfo MemoryAlloc_info = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = memReqs.size,
                memoryTypeIndex = NativeDevice.GetMemoryTypeIndex(memReqs.memoryTypeBits, VkMemoryPropertyFlags.HostVisible),
            };



            vkAllocateMemory(NativeDevice.handle, &MemoryAlloc_info, null, &_memory);
            memory = _memory;

            size = memReqs.size;

        }



        public ulong size;

        public void SetData<T>(params T[] Data) where T : struct
        {

            //vkAllocateMemory(device, &MemoryAlloc_info, null, &_memory);
            //memory = _memory;
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, size, 0, &ppData);

            //Copy Data
            {
                //int stride = Interop.SizeOf<Vertex>();
                //uint size = (uint)(stride * vertices.Length);
                //void* srcPtr = Unsafe.AsPointer(ref Data[0]);
                //Interop.MemoryHelper.CopyBlock(srcPtr, size);
            }

            Interop.MemoryHelper.CopyBlocks<T>(ppData, Data);

            vkUnmapMemory(NativeDevice.handle, memory);

            vkBindBufferMemory(NativeDevice.handle, Handle, memory, 0);


            if (Usage == GraphicsResourceUsage.Staging)
            {

            }


        }




        public void GetData<T>() where T : struct
        {

        }




        public void Dispose()
        {

        }
    }
}
