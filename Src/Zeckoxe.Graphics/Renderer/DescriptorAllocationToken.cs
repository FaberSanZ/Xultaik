// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DescriptorAllocationToken.cs
=============================================================================*/


using Vortice.Vulkan;


namespace Zeckoxe.Graphics
{
    public struct DescriptorAllocationToken
    {
        internal VkDescriptorSet Set;
        internal VkDescriptorPool Pool;

        public DescriptorAllocationToken(VkDescriptorSet set, VkDescriptorPool pool)
        {
            Set = set;
            Pool = pool;
        }



        public ulong GetPoolHandle() => Pool.Handle;
        public ulong GetDescriptorSetHandle() => Set.Handle;
    }
}
