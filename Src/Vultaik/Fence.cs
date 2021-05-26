// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Threading.Tasks;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    public unsafe class Fence : GraphicsResource, IDisposable
    {
        internal VkFence handle;

        public Fence(Device device, bool signaled = true) : base(device)
        {
            VkFenceCreateInfo fenceCreateInfo = new VkFenceCreateInfo()
            {
                sType = VkStructureType.FenceCreateInfo,
                pNext = null,
                flags = signaled ? VkFenceCreateFlags.Signaled : VkFenceCreateFlags.None,
            };


            vkCreateFence(NativeDevice.handle, &fenceCreateInfo, null, out handle);
        }


        public bool IsSignaled => vkGetFenceStatus(NativeDevice.handle, handle) == VkResult.Success;



        public unsafe void Wait()
        {
            fixed (VkFence* ptr = &handle)
            {
                vkWaitForFences(NativeDevice.handle, 1, ptr, true, ulong.MaxValue).CheckResult();
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
                vkResetFences(NativeDevice.handle, 1, ptr).CheckResult();
            }
        }

        public void Dispose()
        {
            vkDestroyFence(NativeDevice.handle, handle, null);
        }
    }
}
