#include "DescriptorAllocator.h"

#include "GraphicsDevice.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    DescriptorAllocator::DescriptorAllocator(GraphicsDevice* graphicsDevice, D3D12_DESCRIPTOR_HEAP_TYPE type, std::uint32_t descriptorCount)
        : m_graphicsDevice(graphicsDevice)
    {
        D3D12_DESCRIPTOR_HEAP_DESC desc{};
        desc.Type = type;
        desc.NumDescriptors = descriptorCount;
        desc.Flags = type == D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV ? D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE : D3D12_DESCRIPTOR_HEAP_FLAG_NONE;

        if (FAILED(m_graphicsDevice->NativeDevice()->CreateDescriptorHeap(&desc, IID_PPV_ARGS(&m_heap))))
        {
            throw std::runtime_error("CreateDescriptorHeap failed.");
        }

        m_descriptorSize = m_graphicsDevice->NativeDevice()->GetDescriptorHandleIncrementSize(type);
    }

    D3D12_CPU_DESCRIPTOR_HANDLE DescriptorAllocator::Allocate(std::uint32_t count)
    {
        D3D12_CPU_DESCRIPTOR_HANDLE handle = m_heap->GetCPUDescriptorHandleForHeapStart();
        handle.ptr += static_cast<SIZE_T>(m_nextIndex) * m_descriptorSize;
        m_nextIndex += count;
        return handle;
    }

    ID3D12DescriptorHeap* DescriptorAllocator::Heap() const noexcept
    {
        return m_heap.Get();
    }
}
