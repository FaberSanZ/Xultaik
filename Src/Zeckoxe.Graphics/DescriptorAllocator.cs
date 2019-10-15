// Copyright(c) 2019-2020 Faber Leonardo.All Rights Reserved.

/*=============================================================================
	DescriptorAllocator.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace Zeckoxe.Graphics
{
    public class DescriptorAllocator : GraphicsResource
    {
        public int Stride { get; }


        internal int DescriptorsPerHeap = 256;
        internal DescriptorHeapType HeapType;
        internal ID3D12DescriptorHeap heap;
        internal CpuDescriptorHandle handle;
        internal int remaining;

        public DescriptorAllocator(GraphicsDevice device, DescriptorHeapType heapType) : base(device)
        {
            HeapType = heapType;
            Stride = GraphicsDevice.NativeDevice.GetDescriptorHandleIncrementSize(heapType);
        }



        public CpuDescriptorHandle Allocate(int count)
        {


            if (heap == null || remaining < count)
            {
                DescriptorHeapDescription descriptorHeapDescription = new DescriptorHeapDescription()
                {
                    Flags = DescriptorHeapFlags.None,
                    Type = HeapType,
                    DescriptorCount = DescriptorsPerHeap,
                    NodeMask = 1,

                };

                heap = GraphicsDevice.NativeDevice.CreateDescriptorHeap(descriptorHeapDescription);

                remaining = DescriptorsPerHeap;

                handle = heap.GetCPUDescriptorHandleForHeapStart();
            }

            CpuDescriptorHandle result = handle;

            handle.Ptr += Stride * count;
            remaining -= count;

            return result;
        }

    }
}
