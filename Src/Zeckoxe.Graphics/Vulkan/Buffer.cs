// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Buffer.cs
=============================================================================*/

using System;
using System.Runtime.CompilerServices;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using Interop = Zeckoxe.Core.Interop;

namespace Zeckoxe.Graphics
{
    // TODO: Vulkan Memory Allocator
    public unsafe class Buffer : GraphicsResource
    {

        internal VkBuffer Handle;
        internal VkBufferView NativeBufferView;
        internal VkAccessFlags NativeAccessMask;
        internal VkDeviceMemory memory;
        internal ulong size;





        public Buffer(GraphicsDevice graphicsDevice, BufferDescription description) : base(graphicsDevice)
        {
            BufferDescription = description;

            Recreate();
        }


        public BufferDescription BufferDescription { get; set; }
        public int SizeInBytes => BufferDescription.SizeInBytes;
        public int ByteStride => BufferDescription.ByteStride;
        public GraphicsResourceUsage Usage => BufferDescription.Usage;
        public BufferFlags Flags => BufferDescription.BufferFlags;





        internal void Recreate()
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
            {
                NativeAccessMask = VkAccessFlags.HostRead | VkAccessFlags.HostWrite;
            }
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



            // Copy vertex data to a buffer visible to the host
            vkCreateBuffer(NativeDevice.handle, &buffer_info, null, out VkBuffer _buffer);
            Handle = _buffer;




            // Allocate memory
            var memoryProperties = VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent;
            if (BufferDescription.Usage == GraphicsResourceUsage.Staging || Usage == GraphicsResourceUsage.Dynamic)
            {
                //memoryProperties = VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent;
            }

            vkGetBufferMemoryRequirements(NativeDevice.handle, Handle, out VkMemoryRequirements memReqs);

            VkMemoryAllocateInfo MemoryAlloc_info = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = memReqs.size,
                memoryTypeIndex = NativeDevice.GetMemoryTypeIndex(memReqs.memoryTypeBits, memoryProperties),
            };



            vkAllocateMemory(NativeDevice.handle, &MemoryAlloc_info, null, &_memory);
            memory = _memory;

            size = memReqs.size;
            vkBindBufferMemory(NativeDevice.handle, Handle, memory, 0);

        }


        public void SetData<T>(ref T Data) where T : unmanaged
        {
            // Map uniform buffer and update it
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, size, 0, &ppData);

            // Copy
            Interop.MemoryHelper.Write(ppData, ref Data);


            // Unmap after data has been copied
            // Note: Since we requested a host coherent memory type for the uniform buffer, the write is instantly visible to the GPU
            vkUnmapMemory(NativeDevice.handle, memory);


        }

        public void SetData<T>(params T[] Data) where T : struct
        {
            switch (Usage)
            {

                case GraphicsResourceUsage.Default:

                    break;

                case GraphicsResourceUsage.Immutable:

                    break;

                case GraphicsResourceUsage.Dynamic:
                    break;

                case GraphicsResourceUsage.Staging:
                    break;

                default:
                    break;
            }

            DynamicData(Data);

        }

        private void DynamicData<T>(params T[] Data) where T : struct
        {
            //vkAllocateMemory(device, &MemoryAlloc_info, null, &_memory);
            //memory = _memory;
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, size, 0, &ppData);

            //Copy Data
            {
                //int stride = Interop.SizeOf<T>();
                //uint size = (uint)(stride * Data.Length);
                //void* srcPtr = Unsafe.AsPointer(ref Data[0]);
                //Interop.MemoryHelper.CopyBlock(srcPtr, size++);
            }

            Interop.MemoryHelper.CopyBlocks<T>(ppData, Data);

            vkUnmapMemory(NativeDevice.handle, memory);

        }


        public void* Map(void* pData)
        {
            vkMapMemory(NativeDevice.handle, memory, 0, size, 0, pData);
            return pData;
        }




        public void GetData<T>() where T : struct
        {

        }




        public void Dispose()
        {

        }
    }
}
