// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vortice.Direct3D12;

namespace Xultaik.Graphics
{
    public class Fence : GraphicsResource
    {

        internal ID3D12Fence fence;
        internal AutoResetEvent fenceEvent;

        public Fence(GraphicsDevice device, long initialValue) : base(device)
        {
            Recreate(initialValue);
        }

        public void Recreate(long initialValue)
        {
            fence = GraphicsDevice.NativeDevice.CreateFence<ID3D12Fence>((ulong)initialValue, FenceFlags.None);
            fenceEvent = new AutoResetEvent(false);
        }

        public void Signal(CommandQueue queue, long fenceValue)
        {
            queue.Queue.Signal(fence, (ulong)fenceValue);
        }

        public void Wait(long fenceValue)
        {
            if (fence.CompletedValue < (ulong)fenceValue)
            {
                fence.SetEventOnCompletion((ulong)fenceValue, fenceEvent);
                fenceEvent.WaitOne();
            }
        }

        public bool IsSignaled(long fenceValue)
        {
            return fence.CompletedValue >= (ulong)fenceValue;
        }

        public void Clear(long fenceValue)
        {
            fence.Signal((ulong)fenceValue);
        }




        //public void Dispose()
        //{
        //    //device.NativeDevice.DeferredRelease(ref fence);
        //    //fenceEvent.Dispose();
        //}
    }
}
