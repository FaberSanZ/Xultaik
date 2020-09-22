// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	DescriptorSet.cs
=============================================================================*/

using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{

    //public unsafe class DescriptorSet
    //{
    //    internal VkDescriptorSet handle;
    //    internal VkDescriptorSet Handle => handle;

    //    internal DescriptorPool pool;
    //    internal List<VkDescriptorSetLayout> descriptorSetLayouts = new List<VkDescriptorSetLayout>();

    //    public DescriptorSet(DescriptorPool descriptorPool)
    //    {
    //        pool = descriptorPool;
    //    }
    //    public DescriptorSet(DescriptorPool descriptorPool, params DescriptorSetLayout[] layouts) : this(descriptorPool)
    //    {

    //        foreach (DescriptorSetLayout layout in layouts)
    //        {
    //            descriptorSetLayouts.Add(layout.handle);
    //        }
    //    }

    //    public void Free()
    //    {
    //        pool.FreeDescriptorSet(this);
    //    }
    //}


    //public sealed unsafe class DescriptorSetLayout : GraphicsResource
    //{
    //    internal VkDescriptorSetLayout handle;

    //    internal VkDescriptorSetLayoutCreateFlags Flags { get; private set; } = 0;
    //    internal VkDescriptorSetLayoutBinding[] Bindings { get; private set; }



    //    public DescriptorSetLayout(GraphicsDevice device, VkDescriptorSetLayoutCreateFlags flags) : base(device)
    //    {
    //        Flags = flags;
    //    }
    //    public DescriptorSetLayout(GraphicsDevice device, params VkDescriptorSetLayoutBinding[] bindings): this(device, 0, bindings)
    //    {
    //    }
    //    public DescriptorSetLayout(GraphicsDevice device, VkDescriptorSetLayoutCreateFlags flags, params VkDescriptorSetLayoutBinding[] bindings) : this(device, flags)
    //    {
    //        Bindings = bindings;

    //        Recreate();
    //    }

    //    public void Recreate()
    //    {
    //        //VkDescriptorSetLayoutBinding* Bindingsptr = stackalloc VkDescriptorSetLayoutBinding[Bindings.Count];

    //        //for (int i = 0; i < Bindings.Length; i++)
    //        //{
    //        //    Bindingsptr[i] = Bindings[i];
    //        //}


    //        VkDescriptorSetLayoutCreateInfo descriptorLayout = new VkDescriptorSetLayoutCreateInfo
    //        {
    //            sType = VkStructureType.DescriptorSetLayoutCreateInfo,
    //            bindingCount = (uint)Bindings.Length,
    //        };

    //        fixed (VkDescriptorSetLayoutBinding* Bindingsptr = Bindings)
    //            descriptorLayout.pBindings = Bindingsptr;

    //        vkCreateDescriptorSetLayout(NativeDevice.handle, &descriptorLayout, null, out handle);

    //    }

    //    public override string ToString()
    //    {
    //        return string.Format($"[0x{handle.Handle.ToString("x")}]");
    //    }


    //}

}
