// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Direct3D12;

namespace Xultaik.Graphics
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

                currentHeap = GraphicsDevice.NativeDevice.CreateDescriptorHeap<ID3D12DescriptorHeap>(descriptorHeapDescription);
                remainingHandles = DescriptorPerHeap;
                currentHandle = currentHeap.GetCPUDescriptorHandleForHeapStart();
            }

            CpuDescriptorHandle result = currentHandle;

            currentHandle.Ptr += (uint)DescriptorSize;
            remainingHandles -= count;

            return result;
        }

    }
}
