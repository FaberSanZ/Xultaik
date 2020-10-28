using System;
using System.Collections.Generic;
using System.Text;

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Zeckoxe.Graphics
{
    public unsafe class Sampler : GraphicsResource
    {

        internal VkSampler handle;
        internal VkImage image;
        internal VkImageLayout imageLayout;
        internal uint mipLevels;

        public Sampler(Device graphicsDevice) : base(graphicsDevice)
        {

        }


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

            // Create image view
            // Textures are not directly accessed by the shaders and
            // are abstracted by image views containing additional
            // information and sub resource ranges
            VkImageViewCreateInfo view = default;
            view.viewType = VkImageViewType.Image2D;
            view.format = default;
            view.components = new VkComponentMapping { r = VkComponentSwizzle.R, g = VkComponentSwizzle.G, b = VkComponentSwizzle.B, a = VkComponentSwizzle.A };
            // The subresource range describes the set of mip levels (and array layers) that can be accessed through this image view
            // It's possible to create multiple image views for a single image referring to different (and/or overlapping) ranges of the image
            view.subresourceRange.aspectMask = VkImageAspectFlags.Color;
            view.subresourceRange.baseMipLevel = 0;
            view.subresourceRange.baseArrayLayer = 0;
            view.subresourceRange.layerCount = 1;
            // Linear tiling usually won't support mip maps
            // Only set mip map count if optimal tiling is used
            //view.subresourceRange.levelCount = (useStaging == 1) ? texture.mipLevels : 1;
            //// The view will be based on the texture's image
            //view.image = texture.image;
            //Util.CheckResult(vkCreateImageView(device, &view, null, out texture.view));
        }
    }
}
