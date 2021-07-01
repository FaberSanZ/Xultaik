
namespace Vultaik
{
    public class MemoryStats
    {
        public MemoryStatInfo[] MemoryType { get; set; } = new MemoryStatInfo[Vortice.Vulkan.Vulkan.MaxMemoryTypes]; // 32

        public MemoryStatInfo[] MemoryHeap { get; set; } = new MemoryStatInfo[Vortice.Vulkan.Vulkan.MaxMemoryHeaps]; // 16

        public MemoryStatInfo Total { get; set; }


        internal MemoryStats(MemoryStatInfo[] memoryTypes, MemoryStatInfo[] memoryHeaps, in MemoryStatInfo total)
        {
            MemoryType = memoryTypes;
            MemoryHeap = memoryHeaps;
            Total = total;
        }

    }
}
