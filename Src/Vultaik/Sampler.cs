using System;
using System.Collections.Generic;
using System.Text;

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    public unsafe class Sampler : GraphicsResource
    {

        internal VkSampler handle;
        internal VkImage image;
        internal VkImageLayout imageLayout;
        internal uint mipLevels;

        public Sampler(Device graphicsDevice) : base(graphicsDevice)
        {
            Init();
        }

        internal VkSampler NativeSampler { get;  set; }

        public void Init()
        {
            // Create sampler
            // In Vulkan textures are accessed by samplers
            // This separates all the sampling information from the 
            // texture data
            // This means you could have multiple sampler objects
            // for the same texture with different settings
            // Similar to the samplers available with OpenGL 3.3
            VkSamplerCreateInfo sampler = default;
            sampler.sType = VkStructureType.SamplerCreateInfo;
            sampler.magFilter = VkFilter.Linear;
            sampler.minFilter = VkFilter.Linear;
            sampler.mipmapMode = VkSamplerMipmapMode.Linear;
            sampler.addressModeU = VkSamplerAddressMode.Repeat;
            sampler.addressModeV = VkSamplerAddressMode.Repeat;
            sampler.addressModeW = VkSamplerAddressMode.Repeat;
            sampler.mipLodBias = 0.0f;
            sampler.compareOp = VkCompareOp.Never;
            sampler.minLod = 0.0f;
            // Set max level-of-detail to mip level count of the texture
            sampler.maxLod = 1;
            // Enable anisotropic filtering
            // This feature is optional, so we must check if it's supported on the Device
            //if (vulkanDevice.features.samplerAnisotropy == 1)
            //{
            //    // Use max. level of anisotropy for this example
            //    sampler.maxAnisotropy = vulkanDevice.properties.limits.maxSamplerAnisotropy;
            //    sampler.anisotropyEnable = True;
            //}

                // The Device does not support anisotropic filtering
                sampler.maxAnisotropy = 1.0f;
                sampler.anisotropyEnable = false;
            
            sampler.borderColor = VkBorderColor.FloatOpaqueWhite;
            vkCreateSampler(NativeDevice.handle, &sampler, null, out handle);

            NativeSampler = handle;
        }
    }
}
