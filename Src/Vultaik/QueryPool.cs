// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	QueryPool.cs   
=============================================================================*/

using System;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    // TODO: QueryPool
    public unsafe class QueryPool : GraphicsResource, IDisposable
    {
        internal VkQueryPool handle;

        public QueryPool(Device device) : base(device)
        {
            QueryCount = 1; 
            VkQueryPoolCreateInfo createInfo = new VkQueryPoolCreateInfo
            {
                sType = VkStructureType.QueryPoolCreateInfo,
                pNext = null,
                queryCount = (uint)QueryCount,
            };
        }

        public int QueryCount { get; }




        public bool TryGetData(long[] dataArray)
        {
            fixed (long* dataPointer = &dataArray[0])
            {
                VkResult result = vkGetQueryPoolResults(NativeDevice.handle, handle, 0, (uint)QueryCount, (uint)QueryCount * 8, dataPointer, 8, VkQueryResultFlags._64);

                if (result == VkResult.NotReady)
                {
                    return false;
                }
            }

            return true;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
