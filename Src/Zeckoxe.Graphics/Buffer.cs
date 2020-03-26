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
        internal VkBuffer NativeBuffer;
        internal VkBufferView NativeBufferView;
        internal VkAccessFlags NativeAccessMask;


        public Buffer(GraphicsDevice graphicsDevice, BufferDescription description) : base(graphicsDevice)
        {
            BufferDescription = description;

            Recreate();
        }

        private void Recreate()
        {
            VkBufferCreateInfo bufferCreateInfo = new VkBufferCreateInfo()
            {
                sType = VkStructureType.BufferCreateInfo,
                pNext = IntPtr.Zero.ToPointer(),
                size = (ulong)BufferDescription.SizeInBytes,
                flags = VkBufferCreateFlags.None,
                //sharingMode = VkSharingMode.Exclusive
            };


            bufferCreateInfo.usage |= VkBufferUsageFlags.TransferSrc;


            if (Usage == GraphicsResourceUsage.Staging)
                NativeAccessMask = VkAccessFlags.HostRead | VkAccessFlags.HostWrite;
            
            else
            {
                if ((Flags/*.HasFlag()*/& BufferFlags.VertexBuffer) != 0)
                {
                    bufferCreateInfo.usage |= VkBufferUsageFlags.VertexBuffer;
                    NativeAccessMask |= VkAccessFlags.VertexAttributeRead;
                }

                if ((Flags & BufferFlags.IndexBuffer) != 0)
                {
                    bufferCreateInfo.usage |= VkBufferUsageFlags.IndexBuffer;
                    NativeAccessMask |= VkAccessFlags.IndexRead;
                }

                if ((Flags & BufferFlags.ConstantBuffer) != 0)
                {
                    bufferCreateInfo.usage |= VkBufferUsageFlags.UniformBuffer;
                    NativeAccessMask |= VkAccessFlags.UniformRead;
                }

                if ((Flags & BufferFlags.ShaderResource) != 0)
                {
                    bufferCreateInfo.usage |= VkBufferUsageFlags.UniformTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderRead;
                }

                if ((Flags & BufferFlags.UnorderedAccess) != 0)
                {
                    bufferCreateInfo.usage |= VkBufferUsageFlags.StorageTexelBuffer;
                    NativeAccessMask |= VkAccessFlags.ShaderWrite;
                }
            }






        }

        public void SetData<T>(params T[] Data) where T : struct
        {


        }




        public void GetData<T>(T[] Data) where T : struct
        {

        }




        public void Dispose()
        {

        }
    }
}
