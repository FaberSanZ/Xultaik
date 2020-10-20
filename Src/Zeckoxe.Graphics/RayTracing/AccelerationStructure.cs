// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AccelerationStructure.cs
=============================================================================*/


using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;


namespace Zeckoxe.Graphics
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

            VkMemoryRequirements2 memoryRequirements2;
            memoryRequirements2.sType = VkStructureType.MemoryRequirements2;

            VkAccelerationStructureMemoryRequirementsInfoKHR accelerationStructureMemoryRequirements = default;
            accelerationStructureMemoryRequirements.sType = VkStructureType.AccelerationStructureMemoryRequirementsInfoKHR;
            accelerationStructureMemoryRequirements.type = VkAccelerationStructureMemoryRequirementsTypeKHR.BuildScratch;
            accelerationStructureMemoryRequirements.buildType = VkAccelerationStructureBuildTypeKHR.Device;
            accelerationStructureMemoryRequirements.accelerationStructure = accelerationStructure;
            vkGetAccelerationStructureMemoryRequirementsKHR(NativeDevice.handle, &accelerationStructureMemoryRequirements, &memoryRequirements2);

            VkBufferCreateInfo bufferCI = new VkBufferCreateInfo
            {
                sType = VkStructureType.BufferCreateInfo,
                size = memoryRequirements2.memoryRequirements.size,
                usage = VkBufferUsageFlags.RayTracingKHR | VkBufferUsageFlags.ShaderDeviceAddress,
                sharingMode = VkSharingMode.Exclusive
            };
            vkCreateBuffer(NativeDevice.handle, &bufferCI, null, out scratchBuffer.buffer);

            vkGetBufferMemoryRequirements(NativeDevice.handle, scratchBuffer.buffer, out VkMemoryRequirements memoryRequirements);

            VkMemoryAllocateFlagsInfo memoryAllocateFI = default;
            memoryAllocateFI.sType = VkStructureType.MemoryAllocateFlagsInfo;
            memoryAllocateFI.flags = VkMemoryAllocateFlags.DeviceAddress;

            VkMemoryAllocateInfo memoryAI = default;
            memoryAI.sType = VkStructureType.MemoryAllocateInfo;
            memoryAI.pNext = &memoryAllocateFI;
            memoryAI.allocationSize = memoryRequirements.size;
            memoryAI.memoryTypeIndex = NativeDevice.GetMemoryType(memoryRequirements.memoryTypeBits, VkMemoryPropertyFlags.DeviceLocal);
            VkDeviceMemory memory = default;
            vkAllocateMemory(NativeDevice.handle, &memoryAI, null, &memory);
            scratchBuffer.memory = memory;

            vkBindBufferMemory(NativeDevice.handle, scratchBuffer.buffer, scratchBuffer.memory, 0);

            VkBufferDeviceAddressInfo buffer_device_address_info = default;
            buffer_device_address_info.sType = VkStructureType.BufferDeviceAddressInfo;
            buffer_device_address_info.buffer = scratchBuffer.buffer;
            scratchBuffer.deviceAddress = (uint)vkGetBufferDeviceAddressKHR(NativeDevice.handle, &buffer_device_address_info); //TODO: vkGetBufferDeviceAddressKHR -> Vortice.Vulkan bug.

            return scratchBuffer;
        }

    }
}
