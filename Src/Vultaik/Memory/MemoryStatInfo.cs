
namespace Vultaik
{
    public struct MemoryStatInfo
    {
        /// Number of `VkDeviceMemory` Vulkan memory blocks allocated.
        public uint BlockCount { get; set; }

        /// Number of #VmaAllocation allocation objects allocated.
        public uint AllocationCount { get; set; }

        /// Number of free ranges of memory between allocations.
        public uint UnusedRangeCount { get; set; }

        /// Total number of bytes occupied by all allocations.
        public ulong UsedBytes { get; set; }

        /// Total number of bytes occupied by unused ranges.
        public ulong UnusedBytes { get; set; }

        public ulong AllocationSizeMin { get; set; }

        public ulong AllocationSizeAvg { get; set; }

        public ulong AllocationSizeMax { get; set; }

        public ulong UnusedRangeSizeMin { get; set; }

        public ulong UnusedRangeSizeAvg { get; set; }

        public ulong UnusedRangeSizeMax { get; set; }
    }
}
