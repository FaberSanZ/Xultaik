// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	DescriptorSetLayout.cs
====================================================================================*/

using Vortice.Vulkan;

namespace Zeckoxe.Vulkan
{
    public class DescriptorSetLayout
    {
        public ShaderStage Stage { get; set; }
        public VkDescriptorType Type { get; set; }
        public int Binding { get; set; }

    }
}
