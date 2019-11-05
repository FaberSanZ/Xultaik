// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	Buffer.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Zeckoxe.Core;

namespace Zeckoxe.Graphics
{
    public class Buffer : GraphicsResource
    {
        private BufferDescription bufferDescription;
        private Vortice.Direct3D12.ResourceDescription nativeDescription;

        public BufferFlags ViewFlags { get; private set; }
        public PixelFormat ViewFormat { get; private set; }
        public object Description { get; private set; }
        public ResourceStates NativeResourceState { get; private set; }
        public object SizeInBytes { get; private set; }

        internal long GPUVirtualAddress;
        private HeapType heapType;

        public Buffer(GraphicsDevice device) : base(device)
        {

        }


        protected Buffer InitializeFromImpl(BufferDescription description, BufferFlags viewFlags, PixelFormat viewFormat, IntPtr dataPointer)
        {
            bufferDescription = description;
            nativeDescription = ConvertToNativeDescription(GraphicsDevice, Description);
            ViewFlags = viewFlags;

            ViewFormat = viewFormat;
            Recreate(dataPointer);


            return this;
        }

        private void Recreate(IntPtr dataPointer)
        {
            var bufferFlags = bufferDescription.BufferFlags;

            if ((bufferFlags & BufferFlags.ConstantBuffer) != 0)
                NativeResourceState |= ResourceStates.VertexAndConstantBuffer;

            if ((bufferFlags & BufferFlags.IndexBuffer) != 0)
                NativeResourceState |= ResourceStates.IndexBuffer;

            if ((bufferFlags & BufferFlags.VertexBuffer) != 0)
                NativeResourceState |= ResourceStates.VertexAndConstantBuffer;

            if ((bufferFlags & BufferFlags.ShaderResource) != 0)
                NativeResourceState |= ResourceStates.PixelShaderResource | ResourceStates.NonPixelShaderResource;

            if ((bufferFlags & BufferFlags.StructuredBuffer) != 0)
            {
                if (bufferDescription.StructureByteStride <= 0)
                    throw new ArgumentException("Element size cannot be less or equal 0 for structured buffer");
            }

            if ((bufferFlags & BufferFlags.ArgumentBuffer) == BufferFlags.ArgumentBuffer)
                NativeResourceState |= ResourceStates.IndirectArgument;


            //GPUVirtualAddress = NativeResource.GPUVirtualAddress;


            if (dataPointer != IntPtr.Zero)
            {
                if (heapType == HeapType.Upload)
                {
                    //var uploadMemory = NativeResource.Map(0);
                    Interop.MemoryHelper.CopyMemory(nameof(dataPointer).GetType(), dataPointer, SizeInBytes);
                    //NativeResource.Unmap(0);
                }
                else
                {
                    //// Copy data in upload heap for later copy
                    //// TODO D3D12 move that to a shared upload heap
                    //SharpDX.Direct3D12.Resource uploadResource;
                    //int uploadOffset;
                    //var uploadMemory = GraphicsDevice.AllocateUploadBuffer(SizeInBytes, out uploadResource, out uploadOffset);
                    //Interop.MemoryHelper.CopyMemory(uploadMemory, dataPointer, SizeInBytes);

                    //// TODO D3D12 lock NativeCopyCommandList usages
                    //var commandList = GraphicsDevice.NativeCopyCommandList;
                    //commandList.Reset(GraphicsDevice.NativeCopyCommandAllocator, null);
                    //// Copy from upload heap to actual resource
                    //commandList.CopyBufferRegion(NativeResource, 0, uploadResource, uploadOffset, SizeInBytes);

                    //// Switch resource to proper read state
                    //commandList.ResourceBarrierTransition(NativeResource, 0, ResourceStates.CopyDestination, NativeResourceState);

                    //commandList.Close();

                    //GraphicsDevice.WaitCopyQueue();
                }
            }

            //NativeShaderResourceView = GetShaderResourceView(ViewFormat);
            //NativeUnorderedAccessView = GetUnorderedAccessView(ViewFormat);

        }



        internal CpuDescriptorHandle GetShaderResourceView(PixelFormat viewFormat)
        {
            var srv = new CpuDescriptorHandle();
            if ((ViewFlags & BufferFlags.ShaderResource) != 0)
            {
                var description = new ShaderResourceViewDescription
                {
                    Shader4ComponentMapping = 0x00001688,
                    Format = ConvertExtensions.ToPixelFormat(viewFormat),
                    ViewDimension = ShaderResourceViewDimension.Buffer,
                    Buffer =
                    {
                        NumElements = 0,
                    }
                };

                if (((ViewFlags & BufferFlags.RawBuffer) == BufferFlags.RawBuffer))
                    description.Buffer.Flags |= BufferShaderResourceViewFlags.Raw;

                srv = GraphicsDevice.ShaderResourceViewAllocator.Allocate(1);
                //GraphicsDevice.CreateShaderResourceView(NativeResource, description, srv);
            }
            return srv;
        }

        private ResourceDescription ConvertToNativeDescription(GraphicsDevice graphicsDevice, object description)
        {
            throw new NotImplementedException();
        }
    }
}
