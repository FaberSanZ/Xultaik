// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Fence.cs
=============================================================================*/


using System;
using System.Threading.Tasks;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class Fence : GraphicsResource, IDisposable
    {
        internal VkFence handle;

        public Fence(GraphicsDevice device, bool signaled = true) : base(device)
        {
            VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo()
            {
                sType = VkStructureType.FenceCreateInfo,
                pNext = null,
                flags = signaled ? VkFenceCreateFlags.Signaled : VkFenceCreateFlags.None,
            };


            vkCreateFence(NativeDevice.Device, &fenceCreateInfo, null, out handle);
        }


        public bool IsSignaled => vkGetFenceStatus(NativeDevice.Device, handle) == VkResult.Success;



        public unsafe void Wait()
        {
            fixed (VkFence* ptr = &handle)
            {
                vkWaitForFences(NativeDevice.Device, 1, ptr, true, ulong.MaxValue).CheckResult();
            }
        }

        public Task WaitAsync()
        {
            return Task.Run(() => Wait());
        }

        public unsafe void Reset()
        {
            fixed (VkFence* ptr = &handle)
            {
                vkResetFences(NativeDevice.Device, 1, ptr).CheckResult();
            }
        }

        public void Dispose()
        {
            vkDestroyFence(NativeDevice.Device, handle, null);
        }
    }
}
