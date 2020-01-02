using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vortice.Direct3D12;

namespace Zeckoxe.Graphics
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
            fence = GraphicsDevice.NativeDevice.CreateFence(initialValue, FenceFlags.None);
            fenceEvent = new AutoResetEvent(false);
        }

        public void Signal(CommandQueue queue, long fenceValue)
        {
            queue.Queue.Signal(fence, fenceValue);
        }

        public void Wait(long fenceValue)
        {
            if (fence.CompletedValue < fenceValue)
            {
                fence.SetEventOnCompletion(fenceValue, fenceEvent);
                fenceEvent.WaitOne();
            }
        }

        public bool IsSignaled(long fenceValue)
        {
            return fence.CompletedValue >= fenceValue;
        }

        public void Clear(long fenceValue)
        {
            fence.Signal(fenceValue);
        }




        //public void Dispose()
        //{
        //    //device.NativeDevice.DeferredRelease(ref fence);
        //    //fenceEvent.Dispose();
        //}
    }
}
