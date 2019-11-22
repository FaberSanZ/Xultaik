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
    public unsafe class Buffer : GraphicsResource
    {
        public BufferFlags ViewFlags { get; private set; }
        public PixelFormat ViewFormat { get; private set; }
        public object Description { get; private set; }
        public ResourceStates NativeResourceState { get; private set; }
        public object SizeInBytes { get; private set; }


        private BufferDescription bufferDescription;
        private Vortice.Direct3D12.ResourceDescription nativeDescription;
        private HeapType heapType;

        internal long GPUVirtualAddress;


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
                if (bufferDescription.StructureByteStride <= 0)
                    throw new ArgumentException("Element size cannot be less or equal 0 for structured buffer");
            

            if ((bufferFlags & BufferFlags.ArgumentBuffer) == BufferFlags.ArgumentBuffer)
                NativeResourceState |= ResourceStates.IndirectArgument;


            //GPUVirtualAddress = NativeResource.GPUVirtualAddress;

            //Span<int> bbs = sta
            if (dataPointer != IntPtr.Zero)
            {
                if (heapType == HeapType.Upload)
                {
                    //var uploadMemory = NativeResource.Map(0);
                    Interop.MemoryHelper.CopyMemory(0, dataPointer, SizeInBytes);
                    //NativeResource.Unmap(0);
                }
                else
                {

                }
            }

            //NativeShaderResourceView = GetShaderResourceView(ViewFormat);
            //NativeUnorderedAccessView = GetUnorderedAccessView(ViewFormat);

        }



        internal CpuDescriptorHandle GetShaderResourceView(PixelFormat viewFormat)
        {
            CpuDescriptorHandle srv = new CpuDescriptorHandle();
            if ((ViewFlags & BufferFlags.ShaderResource) != 0)
            {
                ShaderResourceViewDescription description = new ShaderResourceViewDescription
                {
                    Shader4ComponentMapping = 0x00001688,
                    Format = ConvertExtensions.ToPixelFormat(viewFormat),
                    ViewDimension = ShaderResourceViewDimension.Buffer,
                    Buffer = new BufferShaderResourceView
                    {
                        NumElements = 0,
                    }
                };

                if ((ViewFlags & BufferFlags.RawBuffer) == BufferFlags.RawBuffer)
                    description.Buffer.Flags |= BufferShaderResourceViewFlags.Raw;



                srv = GraphicsDevice.ShaderResourceViewAllocator.Allocate(1);
                //GraphicsDevice.ShaderResourceViewAllocator(NativeResource, description, srv);
            }
            return srv;
        }

        private ResourceDescription ConvertToNativeDescription(GraphicsDevice graphicsDevice, object description)
        {
            throw new NotImplementedException();
        }
    }
}
