// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DeviceLimits.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Vulkan
{
    public class DeviceLimits 
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

    }
}
