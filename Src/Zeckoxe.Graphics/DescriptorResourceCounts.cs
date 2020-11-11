// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	DescriptorResourceCounts.cs
=============================================================================*/



namespace Zeckoxe.Graphics
{
    public struct DescriptorResourceCounts
    {
        public DescriptorResourceCounts(
            uint uniformBufferCount,
            uint sampledImageCount,
            uint samplerCount,
            uint storageBufferCount,
            uint storageImageCount)
        {
            UniformBufferCount = uniformBufferCount;
            SampledImageCount = sampledImageCount;
            SamplerCount = samplerCount;
            StorageBufferCount = storageBufferCount;
            StorageImageCount = storageImageCount;
        }


        public uint UniformBufferCount { get; internal set; }
        public uint SampledImageCount { get; internal set; }
        public uint SamplerCount { get; internal set; }
        public uint StorageBufferCount { get; internal set; }
        public uint StorageImageCount { get; internal set; }
    }
}
