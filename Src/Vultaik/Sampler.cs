using System;
using System.Collections.Generic;
using System.Text;

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vultaik
{
    public class SamplerDescription
    {

        public SamplerDescription()
        {

        }
        public SamplerDescription(float maxLod, float minLod, VkCompareOp compareOp, bool compareEnable, float mipLodBias, 
            VkSamplerAddressMode addressModeW, VkSamplerAddressMode addressModeV, VkSamplerAddressMode addressModeU, 
            VkSamplerMipmapMode mipmapMode, VkFilter minFilter, VkFilter magFilter, VkBorderColor borderColor, bool unNormalizedCoordinates)
        {
            MaxLod = maxLod;
            MinLod = minLod;
            CompareOp = compareOp;
            CompareEnable = compareEnable;
            MipLodBias = mipLodBias;
            AddressModeW = addressModeW;
            AddressModeV = addressModeV;
            AddressModeU = addressModeU;
            MipmapMode = mipmapMode;
            MinFilter = minFilter;
            MagFilter = magFilter;
            BorderColor = borderColor;
            UnNormalizedCoordinates = unNormalizedCoordinates;
        }

        public float MaxLod { get; set; }
        public float MinLod { get; set; }
        public VkCompareOp CompareOp { get; set; }
        public bool CompareEnable { get; set; }
        public float MipLodBias { get; set; }
        public VkSamplerAddressMode AddressModeW { get; set; }
        public VkSamplerAddressMode AddressModeV { get; set; }
        public VkSamplerAddressMode AddressModeU { get; set; }
        public VkSamplerMipmapMode MipmapMode { get; set; }
        public VkFilter MinFilter { get; set; }
        public VkFilter MagFilter { get; set; }
        public VkBorderColor BorderColor { get; set; }
        public bool UnNormalizedCoordinates { get; set; }

        public static SamplerDescription Default = new()
        {
            MagFilter = VkFilter.Linear,
            MinFilter = VkFilter.Linear,
            MipmapMode = VkSamplerMipmapMode.Linear,
            AddressModeU = VkSamplerAddressMode.Repeat,
            AddressModeV = VkSamplerAddressMode.Repeat,
            AddressModeW = VkSamplerAddressMode.Repeat,
            MipLodBias = 0.0f,
            CompareOp = VkCompareOp.Never,
            MinLod = 0.0f,
            MaxLod = 1,
            BorderColor = VkBorderColor.FloatOpaqueWhite,
        };
    }
    // TODO: Using FilterType MinMag
    public enum FilterType
    {
        MinMagMipPoint,

        MinMagPointMipLinear,

        MinPointMagLinearMipPoint,

        MinPointMagMipLinear,

        MinLinearMagMipPoint,

        MinLinearMagPointMipLinear,

        MinMagLinearMipPoint,

        MinMagMipLinear,

        Anisotropic,

        ComparisonMinMagMipPoint,

        ComparisonMinMagPointMipLinear,

        ComparisonMinPointMagLinearMipPoint,

        ComparisonMinPointMagMipLinear,

        ComparisonMinLinearMagMipPoint,

        ComparisonMinLinearMagPointMipLinear,

        ComparisonMinMagLinearMipPoint,

        ComparisonMinMagMipLinear,

        ComparisonAnisotropic,

        MinimumMinMagMipPoint,

        MinimumMinMagPointMipLinear,

        MinimumMinPointMagLinearMipPoint,

        MinimumMinPointMagMipLinear,

        MinimumMinLinearMagMipPoint,

        MinimumMinLinearMagPointMipLinear,

        MinimumMinMagLinearMipPoint,

        MinimumMinMagMipLinear,

        MinimumAnisotropic,

        MaximumMinMagMipPoint,

        MaximumMinMagPointMipLinear,

        MaximumMinPoinyMagLinearMipPoint,

        MaximumMinPointMagMipLinear,

        MaximumMinLinearMagMipPoit,

        MaximumMinLinearMagPointMipLinear,

        MaximumMinMagLinearMipPoint,

        MaximumMinMagMipLinear,

        MaximumAnisotropic,
    }


    public unsafe class Sampler : GraphicsResource
    {

        internal VkSampler handle;


        public Sampler(Device graphicsDevice, SamplerDescription? description = null) : base(graphicsDevice)
        {
            if (description == default || description is null)
                description = SamplerDescription.Default;

            Description = description;
            Recreate();
        }

        public SamplerDescription Description { get; }

        public void Recreate()
        {
            // Create sampler
            // In Vulkan textures are accessed by samplers
            // This separates all the sampling information from the 
            // texture data
            // This means you could have multiple sampler objects
            // for the same texture with different settings
            // Similar to the samplers available with OpenGL 3.3
            VkSamplerCreateInfo sampler = new()
            {
                sType = VkStructureType.SamplerCreateInfo,
                pNext = null,
                flags = VkSamplerCreateFlags.None,

                magFilter = Description.MagFilter,
                minFilter = Description.MinFilter,
                mipmapMode = Description.MipmapMode,
                addressModeU = Description.AddressModeU,
                addressModeV = Description.AddressModeV,
                addressModeW = Description.AddressModeW,
                mipLodBias = Description.MipLodBias,
                compareOp = Description.CompareOp,
                minLod = Description.MinLod,
                borderColor = Description.BorderColor,

                // Set max level-of-detail to mip level count of the texture
                maxLod = Description.MinLod,


                // Enable anisotropic filtering
                // This feature is optional, so we must check if it's supported on the Device
                // Not support anisotropic filtering
                maxAnisotropy = 1.0f,
                anisotropyEnable = false,
            };

            if (NativeDevice.SamplerAnisotropySupport)
            {
                // Use max. level of anisotropy for this example
                sampler.maxAnisotropy = NativeDevice.NativeAdapter.device_properties.limits.maxSamplerAnisotropy;
                sampler.anisotropyEnable = true;
            }



            vkCreateSampler(NativeDevice.handle, &sampler, null, out handle);

        }
    }
}
