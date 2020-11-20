// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


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

        internal VkBuffer handle;
        internal VkBufferView NativeBufferView;
        internal VkAccessFlags NativeAccessMask;
        internal VkDeviceMemory memory;
        internal ulong size;



        internal IntPtr mappedData;


        public Buffer(Device graphicsDevice, BufferDescription description) : base(graphicsDevice)
        {
            BufferDescription = description;

            Recreate();
        }


        public IntPtr MappedData => mappedData;

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

                if ((Flags & BufferFlags.IndexBuffer) is not 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.IndexBuffer;
                    NativeAccessMask |= VkAccessFlags.IndexRead;
                }

                if ((Flags & BufferFlags.ConstantBuffer) is not 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.UniformBuffer;
                    NativeAccessMask |= VkAccessFlags.UniformRead;
                }

                if ((Flags & BufferFlags.ShaderResource) is not 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.UniformTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderRead;
                }

                if ((Flags & BufferFlags.UnorderedAccess) is not 0)
                {
                    buffer_info.usage |= VkBufferUsageFlags.StorageTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderWrite;
                }
            }

            vkCreateBuffer(NativeDevice.handle, &buffer_info, null, out handle);




            // Allocate memory
            var memoryProperties = VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent;
            if (BufferDescription.Usage is GraphicsResourceUsage.Staging || Usage is GraphicsResourceUsage.Dynamic)
            {
                //memoryProperties = VkMemoryPropertyFlags.HostVisible | VkMemoryPropertyFlags.HostCoherent;
            }

            vkGetBufferMemoryRequirements(NativeDevice.handle, handle, out VkMemoryRequirements memReqs);

            VkMemoryAllocateInfo MemoryAlloc_info = new VkMemoryAllocateInfo()
            {
                sType = VkStructureType.MemoryAllocateInfo,
                pNext = null,
                allocationSize = memReqs.size,
                memoryTypeIndex = NativeDevice.GetMemoryTypeIndex(memReqs.memoryTypeBits, memoryProperties),
            };


            VkDeviceMemory _memory;
            vkAllocateMemory(NativeDevice.handle, &MemoryAlloc_info, null, &_memory);
            memory = _memory;

            size = memReqs.size;
            vkBindBufferMemory(NativeDevice.handle, handle, memory, 0);

        }


        public void SetData<T>(ref T Data) where T : unmanaged
        {
            // Map uniform buffer and update it
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, (ulong)BufferDescription.SizeInBytes, 0, &ppData);

            // Copy
            Interop.MemoryHelper.Write(ppData, ref Data);


            // Unmap after data has been copied
            // Note: Since we requested a host coherent memory type for the uniform buffer, the write is instantly visible to the GPU
            vkUnmapMemory(NativeDevice.handle, memory);


        }



        public void Map(ulong offset = 0)
        {
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, offset, (ulong)BufferDescription.SizeInBytes, 0, &ppData);
            mappedData = (IntPtr)ppData;
        }
        public void Unmap()
        {

            vkUnmapMemory(NativeDevice.handle, memory);
            mappedData = IntPtr.Zero;
        }



        public void SetData<T>(Span<T> Data) where T : struct
        {
            DynamicData(Data);
        }


        public void SetData<T>(T[] Data) where T : unmanaged
        {
            switch (Usage)
            {

                case GraphicsResourceUsage.Default:

                    break;

                case GraphicsResourceUsage.Immutable:

                    break;

                case GraphicsResourceUsage.Dynamic:
                    DynamicData(Data);

                    break;

                case GraphicsResourceUsage.Staging:
                    break;

                default:
                    break;
            }


            

        }


        private void DynamicData<T>(Span<T> Data) where T : struct
        {
            //vkAllocateMemory(device, &MemoryAlloc_info, null, &_memory);
            //memory = _memory;
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, (ulong)BufferDescription.SizeInBytes, 0, &ppData);

            //Copy Data
            {
                //int stride = Interop.SizeOf<T>();
                //uint size = (uint)(stride * Data.Length);
                //void* srcPtr = Unsafe.AsPointer(ref Data[0]);
                //Interop.MemoryHelper.CopyBlock(srcPtr, size++);
            }

            Data.CopyTo(new Span<T>(ppData, Data.Length));

            vkUnmapMemory(NativeDevice.handle, memory);

        }


        private void DynamicData<T>(T[] Data) where T : struct
        {
            //vkAllocateMemory(device, &MemoryAlloc_info, null, &_memory);
            //memory = _memory;
            void* ppData;
            vkMapMemory(NativeDevice.handle, memory, 0, (ulong)BufferDescription.SizeInBytes, 0, &ppData);

            //Copy Data
            {
                //int stride = Interop.SizeOf<T>();
                //uint size = (uint)(stride * Data.Length);
                //void* srcPtr = Unsafe.AsPointer(ref Data[0]);
            }

            Interop.MemoryHelper.CopyBlocks<T>(ppData, Data);



            vkUnmapMemory(NativeDevice.handle, memory);

        }


        public void* Map(void* pData)
        {
            vkMapMemory(NativeDevice.handle, memory, 0, (ulong)BufferDescription.SizeInBytes, 0, pData);
            return pData;
        }




        public void GetData<T>() where T : struct
        {

        }




        public void Dispose()
        {

        }

        public void CopyTo(CommandBuffer cmd, Buffer buff, ulong size = 0, ulong srcOffset = 0, ulong dstOffset = 0)
        {

            VkBufferCopy bufferCopy = new VkBufferCopy
            {
                size = size,
                srcOffset = srcOffset,
                dstOffset = dstOffset
            };
            vkCmdCopyBuffer(cmd.handle, handle, buff.handle, 1, &bufferCopy);
            
        }
    }
}
