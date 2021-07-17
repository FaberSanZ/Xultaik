// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Vultaik
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


    public class ResourceDataBindless
    {

        public ResourceDataBindless()
        {

        }

        public Buffer[] Buffer { get; set; }
        public Image[] Images { get; set; }
        public Sampler[] Sampler { get; set; }


        public int Offset { get; set; }
        public int Binding { get; set; }


        public VkDescriptorType DescriptorType { get; set; }

    }


    public class DescriptorData
    {
        public DescriptorData()
        {

        }

        public List<ResourceData> Data = new();
        public List<ResourceDataBindless> DataBindless = new();



        public void SetBindlessImage(int _binding, Image[] image)
        {
            DataBindless.Add(new ResourceDataBindless 
            {
                Binding = _binding,
                Images = image,
                DescriptorType = VkDescriptorType.SampledImage,
            });

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public void SetStructuredBuffer(int binding, Buffer buffer, int offset = 0)
        {

            Data.Add(new()
            {
                Offset = offset,
                Binding = binding,
                Buffer = buffer,
                DescriptorType = VkDescriptorType.StorageBuffer,
            });
        }


        public void SetReadWriteImage(int _binding, Image image)
        {

            Data.Add(new()
            {
                Binding = _binding,
                Texture = image,
                DescriptorType = VkDescriptorType.StorageImage,
            });
        }

        public void SetImage(int _binding, Image image)
        {

            Data.Add(new()
            {
                Binding = _binding,
                Texture = image,
                DescriptorType = VkDescriptorType.SampledImage,
            });
        }


        public void SetSampler(int _binding, Sampler sampler)
        {

            Data.Add(new()
            {
                Binding = _binding,
                Sampler = sampler,
                DescriptorType = VkDescriptorType.Sampler,
            });
        }



        public void SetImageSampler(int _binding, Image image, Sampler sampler)
        {

            Data.Add(new()
            {
                Binding = _binding,
                Sampler = sampler,
                Texture = image,
                DescriptorType = VkDescriptorType.CombinedImageSampler,
            });
        }


        public void SetUniformBufferDynamic(int binding, Buffer buffer, int offset = 0)
        {
            Data.Add(new()
            {
                Offset = offset,
                Binding = binding,
                Buffer = buffer,
                DescriptorType = VkDescriptorType.UniformBufferDynamic,
            });

        }

        public void SetUniformBuffer(int binding, Buffer buffer, int offset = 0)
        {
            Data.Add(new()
            {
                Offset = offset,
                Binding = binding,
                Buffer = buffer,
                DescriptorType = VkDescriptorType.UniformBuffer,
            });

        }


    }
}
