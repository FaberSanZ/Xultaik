// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;
using Vortice.DXGI;

namespace Xultaik.Graphics
{
    public unsafe class Buffer : GraphicsResource
    {
        public BufferDescription Description { get; private set; }

        public int SizeInBytes => Description.SizeInBytes;

        //public BufferFlags Flags => BufferFlags. Description.Flags;

        public HeapType HeapType => Description.HeapType;

        public int StructureByteStride => Description.StructureByteStride;


        internal ID3D12Resource NativeResource;
        internal CpuDescriptorHandle? constantBufferView;
        internal long GPUVirtualAddress;



        public Buffer(GraphicsDevice device, BufferDescription description) : base(device)
        {
            InitializeFrom(description);
        }

        public void SetData<T>(T[] data, int offsetInBytes = 0) where T : unmanaged
        {

            if (HeapType == HeapType.Upload)
            {
                IntPtr mappedResource = IntPtr.Zero;
                NativeResource.Map(0, null, mappedResource.ToPointer());

                Interop.MemoryHelper.Write(mappedResource, data);

                NativeResource.Unmap(0);
            }
            else
            {

                //IntPtr mappedResource = NativeResource.Map(0);

                //Interop.MemoryHelper.Write(mappedResource, data);

                //NativeResource.Unmap(0);
                //mappedResource = IntPtr.Zero;


                //CommandList CommandList = new CommandList(GraphicsDevice);
                //CommandList.Reset();
                //var commandList = CommandList.nativeCommandList;

                ////commandList.Reset();
                //// Copy from upload heap to actual resource
                //commandList.CopyBufferRegion(NativeResource, 0, uploadResource, uploadOffset, SizeInBytes);

                //// Switch resource to proper read state
                //commandList.ResourceBarrierTransition(NativeResource, 0, ResourceStates.CopyDestination, NativeResourceState);

                //commandList.Close();

                //GraphicsDevice.WaitCopyQueue();
            }
        }


        public void InitializeFrom(BufferDescription description)
        {
            Description = description;

            ResourceStates resourceStates = ResourceStates.Common;



            if (description.HeapType == HeapType.Upload)
                resourceStates |= ResourceStates.GenericRead;
            

            else if (description.HeapType == HeapType.Readback)
                resourceStates |= ResourceStates.CopyDest;


            if ((description.Flags & BufferFlags.ConstantBuffer) != 0)
                constantBufferView = CreateConstantBufferView();






            ResourceDescription ResourceDesc = new ResourceDescription()
            {
                Width = (ulong)SizeInBytes,
                Height = 1,
                DepthOrArraySize = 1,
                Dimension = ResourceDimension.Buffer,
                Alignment = 65536,
                Layout = TextureLayout.RowMajor,
                Flags = ResourceFlags.None,
                MipLevels = 1,
                Format = Format.Unknown,
                SampleDescription = new SampleDescription()
                {
                    Count = 1,
                    Quality = 0
                }
            };




            HeapProperties heapProp = new HeapProperties()
            {
                Type = (Vortice.Direct3D12.HeapType)description.HeapType,
                CreationNodeMask = 1,
                VisibleNodeMask = 1,
                CPUPageProperty = CpuPageProperty.Unknown,
                MemoryPoolPreference = MemoryPool.Unknown,

            };

            NativeResource = GraphicsDevice.NativeDevice.CreateCommittedResource<ID3D12Resource>(heapProp, HeapFlags.None, ResourceDesc, resourceStates);







            GPUVirtualAddress = (long)NativeResource.GPUVirtualAddress;


            //return InitializeFrom(resource, description);
        }


        internal Buffer InitializeFrom(ID3D12Resource resource, BufferDescription description, int firstElement = 0, int elementCount = 0)
        {


            return this;
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

    }

    
}
