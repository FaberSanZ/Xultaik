// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Zeckoxe.Vulkan
{

    public class ResourceData
    {

        public ResourceData()
        {

        }

        public Buffer Buffer { get; set; }
        public Image Texture { get; set; }
        public Sampler Sampler { get; set; }


        public int Offset { get; set; }
        public int Binding { get; set; }


        public VkDescriptorType DescriptorType { get; set; }

    }


    public class DescriptorData
    {
        public DescriptorData()
        {

        }

        public List<ResourceData> resourceInfos = new();








        public void SetImageSampler(int _binding, Image image, Sampler sampler)
        {

            resourceInfos.Add(new()
            {
                Binding = _binding,
                Sampler = sampler,
                Texture = image,
                DescriptorType = VkDescriptorType.CombinedImageSampler,
            });
        }


        public void SetUniformBuffer(int binding, Buffer buffer, int offset = 0)
        {
            resourceInfos.Add(new()
            {
                Offset = offset,
                Binding = binding,
                Buffer = buffer,
                DescriptorType = VkDescriptorType.UniformBuffer,
            });

        }


    }
}
