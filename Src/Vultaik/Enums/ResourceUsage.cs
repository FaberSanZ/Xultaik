// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




namespace Vultaik
{
    public enum ResourceUsage
    {
        /// <summary>
        /// No Intended memory usage specified
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Memory will be used on device only, so fast access from the device is preferred.
        /// It usually means device-local GPU (video) memory.
        /// No need to be mappable on host.
        /// </summary>
        /// <remarks>
        /// Usage:
        ///     - Resources written and read by device, e.g. images used as attachments.
        ///     - Resources transferred from host once (immutable) or infrequently and read by
        ///       device multiple times, e.g. textures to be sampled, vertex buffers, uniform
        ///       (constant) buffers, and majority of other types of resources used on GPU.
        /// 
        /// Allocation may still end up in `HOST_VISIBLE` memory on some implementations.
        /// In such case, you are free to map it.
        /// You can use #AllocationCreateMapped with this usage type.
        /// </remarks>
        GPU_Only,

        /// <summary>
        /// Memory will be mappable on host.
        /// It usually means CPU (system) memory.
        /// Guarantees to be `HOST_VISIBLE` and `HOST_COHERENT`.
        /// CPU access is typically uncached. Writes may be write-combined.
        /// Resources created in this pool may still be accessible to the device, but access to them can be slow.
        /// </summary>
        /// <remarks>
        /// Usage: Staging copy of resources used as transfer source.
        /// </remarks>
        CPU_Only,

        /// <summary>
        /// Memory that is mappable on host (guarantees to be `HOST_VISIBLE`) and preferably fast to access by GPU.
        /// CPU access is typically uncached. Writes may be write-combined.
        /// </summary>
        /// <remarks>
        /// Usage: Resources written frequently by host (dynamic), read by device. E.g. textures, vertex buffers, uniform buffers updated every frame or every draw call.
        /// </remarks>
        CPU_To_GPU,

        /// <summary>
        /// Memory mappable on host (guarantees to be `HOST_VISIBLE`) and cached.
        /// It is roughly equivalent of `D3D12_HEAP_TYPE_READBACK`.
        /// </summary>
        /// <remarks>
        /// Usage:
        ///     - Resources written by device, read by host - results of some computations, e.g. screen capture, average scene luminance for HDR tone mapping.
        ///     - Any resources read or accessed randomly on host, e.g. CPU-side copy of vertex buffer used as source of transfer, but also used for collision detection.
        /// </remarks>
        GPU_To_CPU,

        /// <summary>
        /// CPU memory - memory that is preferably not `DEVICE_LOCAL`, but also not guaranteed to be `HOST_VISIBLE`.
        /// </summary>
        /// <remarks>
        /// Usage: Staging copy of resources moved from GPU memory to CPU memory as part
        /// of custom paging/residency mechanism, to be moved back to GPU memory when needed.
        /// </remarks>
        CPU_Copy,

        /// <summary>
        /// Lazily allocated GPU memory having `VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT`.
        /// Exists mostly on mobile platforms. Using it on desktop PC or other GPUs with no such memory type present will fail the allocation.
        /// Allocations with this usage are always created as dedicated - it implies #VMA_ALLOCATION_CREATE_DEDICATED_MEMORY_BIT.
        /// </summary>
        /// <remarks>
        /// Usage: Memory for transient attachment images (color attachments, depth attachments etc.), created with `IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT`.
        /// </remarks>
        GPU_LazilyAllocated
    }
}