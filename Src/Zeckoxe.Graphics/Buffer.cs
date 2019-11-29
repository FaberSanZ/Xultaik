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
        public IntPtr MappedResource { get; private set; }

        public int FirstElement { get; set; }

        public int ElementCount { get; set; }

        public BufferDescription Description { get; private set; }

        public int SizeInBytes => Description.SizeInBytes;

        public BufferFlags Flags => Description.Flags;

        public HeapType HeapType => Description.HeapType;

        public int StructureByteStride => Description.StructureByteStride;




        //internal
        internal Vortice.Direct3D12.ResourceDescription nativeDescription;
        internal ID3D12Resource NativeResource;
        internal CpuDescriptorHandle? shaderResourceView;
        internal CpuDescriptorHandle? constantBufferView;
        internal CpuDescriptorHandle? unorderedAccessView;


        public Buffer(GraphicsDevice device, BufferDescription description) : base(device)
        {
            nativeDescription = ConvertToNativeDescription(description);
            InitializeFrom(description);
        }

        public void SetData<T>(T[] data, int offsetInBytes = 0) where T : unmanaged
        {
            offsetInBytes = FirstElement * StructureByteStride + offsetInBytes;

            if (HeapType == HeapType.Upload)
            {
                Map(0);
                Interop.MemoryHelper.Write(MappedResource + offsetInBytes, data);
                Unmap(0);
            }
            else
            {
                // Copy data in upload heap for later copy
                // TODO D3D12 move that to a shared upload heap
            }
        }


        public Buffer InitializeFrom(BufferDescription description)
        {
            ResourceStates resourceStates = ResourceStates.Common;


            if ((description.Flags & BufferFlags.ConstantBuffer) != 0)
                resourceStates |= ResourceStates.VertexAndConstantBuffer;


            if ((description.Flags & BufferFlags.IndexBuffer) != 0)
                resourceStates |= ResourceStates.IndexBuffer;


            if ((description.Flags & BufferFlags.VertexBuffer) != 0)
                resourceStates |= ResourceStates.VertexAndConstantBuffer;


            HeapProperties heapProp = new HeapProperties()
            {
                Type = (Vortice.Direct3D12.HeapType)description.HeapType,
                CreationNodeMask = 1,
                VisibleNodeMask = 1
            };

            ID3D12Resource resource = GraphicsDevice.NativeDevice.CreateCommittedResource(heapProp, HeapFlags.None, nativeDescription, resourceStates);

            return InitializeFrom(resource, description);
        }


        internal Buffer InitializeFrom(ID3D12Resource resource, BufferDescription description, int firstElement = 0, int elementCount = 0)
        {
            NativeResource = resource;
            Description = description;

            FirstElement = firstElement;

            if (description.StructureByteStride != 0)
                ElementCount = elementCount == 0 ? description.SizeInBytes / description.StructureByteStride : elementCount;
            


            if ((description.Flags &  BufferFlags.ConstantBuffer) != 0)
                constantBufferView = CreateConstantBufferView();
            

            if ((description.Flags & BufferFlags.ShaderResource) != 0)
                shaderResourceView = CreateShaderResourceView();
            

            if ((description.Flags & BufferFlags.UnorderedAccess) != 0)
                unorderedAccessView = CreateUnorderedAccessView();
            

            return this;
        }


        internal ResourceDescription ConvertToNativeDescription(BufferDescription description)
        {
            ResourceFlags flags = ResourceFlags.None;
            int size = description.SizeInBytes;

            if ((description.Flags & BufferFlags.UnorderedAccess) != 0)
                flags |= ResourceFlags.AllowUnorderedAccess;


            size = (size + 256) & ~256; 

            return ResourceDescription.Buffer(size, flags);
        }




        public IntPtr Map(int subresource)
        {
            IntPtr mappedResource = NativeResource?.Map(subresource) ?? throw new InvalidOperationException();
            MappedResource = mappedResource;
            return mappedResource;
        }

        public void Unmap(int subresource)
        {
            NativeResource?.Unmap(subresource);
            MappedResource = IntPtr.Zero;
        }


        internal CpuDescriptorHandle CreateConstantBufferView()
        {
            CpuDescriptorHandle cpuHandle = GraphicsDevice.ShaderResourceViewAllocator.Allocate(1);

            int constantBufferSize = (SizeInBytes + 255) & ~255;

            ConstantBufferViewDescription cbvDescription = new ConstantBufferViewDescription()
            {
                BufferLocation = NativeResource!.GPUVirtualAddress,
                SizeInBytes = constantBufferSize
            };

            GraphicsDevice.NativeDevice.CreateConstantBufferView(cbvDescription, cpuHandle);

            return cpuHandle;
        }

        internal CpuDescriptorHandle CreateShaderResourceView()
        {
            CpuDescriptorHandle cpuHandle = GraphicsDevice.ShaderResourceViewAllocator.Allocate(1);

            ShaderResourceViewDescription description = new ShaderResourceViewDescription
            {
                Shader4ComponentMapping = DefaultComponentMapping(),
                ViewDimension = ShaderResourceViewDimension.Buffer,
                Buffer = new BufferShaderResourceView()
                {
                    FirstElement = FirstElement,
                    NumElements = ElementCount,
                    StructureByteStride = StructureByteStride
                },
            };

            GraphicsDevice.NativeDevice.CreateShaderResourceView(NativeResource, description, cpuHandle);

            return cpuHandle;
        }

        internal CpuDescriptorHandle CreateUnorderedAccessView()
        {
            CpuDescriptorHandle cpuHandle = GraphicsDevice.ShaderResourceViewAllocator.Allocate(1);

            UnorderedAccessViewDescription description = new UnorderedAccessViewDescription
            {
                ViewDimension = UnorderedAccessViewDimension.Buffer,
                Buffer = new BufferUnorderedAccessView()
                {
                    FirstElement = FirstElement,
                    NumElements = ElementCount,
                    StructureByteStride = StructureByteStride
                }
            };

            GraphicsDevice.NativeDevice.CreateUnorderedAccessView(NativeResource, null, description, cpuHandle);

            return cpuHandle;
        }
    }

    
}
