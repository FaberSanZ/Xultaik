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

        public MultisampleCount MultisampleCount => (MultisampleCount)Tools.ExtractMaxSampleCount(NativeDevice.NativeAdapter.Properties);

        public PixelFormat SupportedDepthFormat => NativeDevice.NativeAdapter.GetSupportedDepthFormat(new List<PixelFormat>()
        {
                PixelFormat.D32SfloatS8Uint,
                PixelFormat.D32Sfloat,
                PixelFormat.D24UnormS8Uint,
                PixelFormat.D16UnormS8Uint,
                PixelFormat.D16Unorm,
        });


        public DeviceLimits(GraphicsDevice device) : base(device) { }



    }
}
