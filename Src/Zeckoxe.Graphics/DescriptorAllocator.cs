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
        private const int DescriptorPerHeap = 256;
        private DescriptorHeapType DescriptorHeapType;
        private ID3D12DescriptorHeap currentHeap;
        private CpuDescriptorHandle currentHandle;
        private int remainingHandles;
        private readonly int DescriptorSize;

        public DescriptorAllocator(GraphicsDevice device, DescriptorHeapType descriptorHeapType) : base(device)
        {
            DescriptorHeapType = descriptorHeapType;
            DescriptorSize = device.NativeDevice.GetDescriptorHandleIncrementSize(descriptorHeapType);
        }

        public void Dispose()
        {
            currentHeap?.Dispose();
            currentHeap = null;
        }

        internal CpuDescriptorHandle Allocate(int count)
        {
            if (currentHeap == null || remainingHandles < count)
            {

                DescriptorHeapDescription descriptorHeapDescription = new DescriptorHeapDescription()
                {
                    Flags = DescriptorHeapFlags.None,
                    Type = DescriptorHeapType,
                    DescriptorCount = DescriptorPerHeap,
                    NodeMask = 1,
                };

                currentHeap = GraphicsDevice.NativeDevice.CreateDescriptorHeap(descriptorHeapDescription);
                remainingHandles = DescriptorPerHeap;
                currentHandle = currentHeap.GetCPUDescriptorHandleForHeapStart();
            }

            CpuDescriptorHandle result = currentHandle;

            currentHandle.Ptr += DescriptorSize;
            remainingHandles -= count;

            return result;
        }

    }
}
