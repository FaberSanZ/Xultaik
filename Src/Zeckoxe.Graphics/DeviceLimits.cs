// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	DeviceLimits.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public class DeviceLimits : GraphicsResource
    {
        public int DescriptorSets => 8;

        public int Bindings => 16;

        public int BindingsBindlessVarying => 64 * 1024;

        public int BindingsBindless => 4 * 1024;

        public int Attachments => 8;

        public int VertexAttribs => 16;

        public int VertexBuffers => 4;

        public int ConstantSize => 128;

        public int MaxUboSize => 16 * 1024;

        public int SpecConstants => 8;

        public float TimestampPeriod => NativeDevice.NativeAdapter.Properties.limits.timestampPeriod;

        public int MaxDrawIndirectCount => (int)NativeDevice.NativeAdapter.Properties.limits.maxDrawIndirectCount;

        public MultisampleCount MultisampleCount => VulkanConvert.ToSampleCountFlags_Invert(Tools.ExtractMaxSampleCount(NativeDevice.NativeAdapter.Properties));





        public DeviceLimits(GraphicsDevice device) : base(device) { }



        //public string ExtractVersion(uint _value)
        //{

        //    uint major = _value >> 22;
        //    uint minor = (_value >> 12) & 0x03FF;
        //    uint patch = _value & 0x0FFF;

        //    return $"{major}.{minor}.{patch}";
        //}
    }
}
