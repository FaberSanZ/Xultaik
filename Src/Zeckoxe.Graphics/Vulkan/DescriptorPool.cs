using System;
using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class DescriptorPool //: GraphicsResource
    {
        //internal VkDescriptorPool handle;
        //internal uint MaxSets;
        //internal List<VkDescriptorPoolSize> pool_sizes;

        //private DescriptorPool() : base(null) { }

        //public DescriptorPool(GraphicsDevice device, uint maxSets = 1) : base(device)
        //{
        //    MaxSets = maxSets;
        //    pool_sizes = new List<VkDescriptorPoolSize>();

        //    Recreate();

        //}

        //public void Recreate()
        //{

        //    VkDescriptorPoolSize* pool_sizesptr = stackalloc VkDescriptorPoolSize[pool_sizes.Count];

        //    for (int i = 0; i < pool_sizes.Count; i++)
        //    {
        //        pool_sizesptr[i] = pool_sizes[i];
        //    }


        //    VkDescriptorPoolCreateInfo descriptorPoolInfo = new VkDescriptorPoolCreateInfo
        //    {
        //        sType = VkStructureType.DescriptorPoolCreateInfo,
        //        poolSizeCount = (uint)pool_sizes.Count,
        //        pPoolSizes = pool_sizesptr,
        //        // Set the max. number of descriptor sets that can be requested from this pool (requesting beyond this limit will result in an error)
        //        maxSets = MaxSets
        //    };

        //    vkCreateDescriptorPool(NativeDevice.handle, &descriptorPoolInfo, null, out handle);

        //}

        //public DescriptorSet Allocate(params DescriptorSetLayout[] layouts)
        //{
        //    DescriptorSet ds = new DescriptorSet(this, layouts);
        //    Allocate(ds);
        //    return ds;
        //}
        //public void Allocate(DescriptorSet descriptorSet)
        //{
        //    //VkDescriptorSetAllocateInfo allocInfo = VkDescriptorSetAllocateInfo.New();
        //    //allocInfo.descriptorPool = handle;
        //    //allocInfo.descriptorSetCount = (uint)descriptorSet.descriptorSetLayouts.Count;
        //    //allocInfo.pSetLayouts = descriptorSet.descriptorSetLayouts;

        //    //vkAllocateDescriptorSets(NativeDevice.handle, ref allocInfo, &descriptorSet.handle);

        //}
        //public void FreeDescriptorSet(params DescriptorSet[] descriptorSets)
        //{
        //    if (descriptorSets.Length == 1)
        //    {
        //        //vkFreeDescriptorSets(NativeDevice.handle, handle, 1, &descriptorSets[0].handle);
        //        return;
        //    }
        //    //vkFreeDescriptorSets(Dev.VkDev, handle, (uint)descriptorSets.Length, descriptorSets);
        //}
        //public void Reset()
        //{
        //    vkResetDescriptorPool(NativeDevice.handle, handle, 0);
        //}

        //public override string ToString()
        //{
        //    return string.Format($"{base.ToString()}[0x{handle.Handle.ToString("x")}]");
        //}

        //public void Dispose(bool disposing)
        //{
        //    vkDestroyDescriptorPool(NativeDevice.handle, handle, null);
        //}
    }
}
