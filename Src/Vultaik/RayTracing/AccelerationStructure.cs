// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Vultaik
{



    internal class RayTracingScratchBuffer
    {
        internal uint deviceAddress = 0;
        internal VkBuffer buffer = VkBuffer.Null;
        internal VkDeviceMemory memory = VkDeviceMemory.Null;
    };

    // Holds data for a memory object bound to an acceleration structure
    internal class RayTracingObjectMemory
    {
        internal uint deviceAddress = 0;
        internal VkDeviceMemory memory = VkDeviceMemory.Null;
    };

    public unsafe class AccelerationStructure : GraphicsResource
    {
        internal VkAccelerationStructureKHR accelerationStructure;
        internal uint handle;
        internal RayTracingObjectMemory objectMemory;

        internal const int INDEX_RAYGEN_GROUP = 0;
        internal const int INDEX_MISS_GROUP = 1;
        internal const int INDEX_CLOSEST_HIT_GROUP = 2;


        public AccelerationStructure(Device graphicsDevice) : base(graphicsDevice)
        {

        }



        internal RayTracingScratchBuffer create_scratch_buffer(VkAccelerationStructureKHR accelerationStructure)
        {
            RayTracingScratchBuffer scratchBuffer = default;

            return scratchBuffer;
        }

    }
}
