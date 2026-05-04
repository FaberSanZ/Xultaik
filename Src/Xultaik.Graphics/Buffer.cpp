#include "Buffer.h"

#include "GraphicsDevice.h"

#include <stdexcept>
#include <cstring>

namespace Xultaik::Graphics
{
    Buffer::Buffer(GraphicsDevice* graphicsDevice, const BufferDescription& description)
        : GraphicsResource(graphicsDevice),
          m_description(description)
    {
        D3D12_HEAP_PROPERTIES heapProps{};
        heapProps.Type = static_cast<D3D12_HEAP_TYPE>(m_description.heapType);
        heapProps.CPUPageProperty = D3D12_CPU_PAGE_PROPERTY_UNKNOWN;
        heapProps.MemoryPoolPreference = D3D12_MEMORY_POOL_UNKNOWN;

        D3D12_RESOURCE_DESC resourceDesc{};
        resourceDesc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
        resourceDesc.Width = m_description.sizeInBytes;
        resourceDesc.Height = 1;
        resourceDesc.DepthOrArraySize = 1;
        resourceDesc.MipLevels = 1;
        resourceDesc.Format = DXGI_FORMAT_UNKNOWN;
        resourceDesc.SampleDesc.Count = 1;
        resourceDesc.Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR;

        if (FAILED(graphicsDevice->NativeDevice()->CreateCommittedResource(
            &heapProps,
            D3D12_HEAP_FLAG_NONE,
            &resourceDesc,
            D3D12_RESOURCE_STATE_COMMON,
            nullptr,
            IID_PPV_ARGS(&m_resource))))
        {
            throw std::runtime_error("CreateCommittedResource failed.");
        }
    }

    void Buffer::SetData(const void* data, std::size_t sizeInBytes)
    {
        if (m_description.heapType != HeapType::Upload)
        {
            return;
        }

        void* mapped = nullptr;
        D3D12_RANGE readRange{0, 0};
        m_resource->Map(0, &readRange, &mapped);
        std::memcpy(mapped, data, sizeInBytes);
        m_resource->Unmap(0, nullptr);
    }

    ID3D12Resource* Buffer::Native() const noexcept
    {
        return m_resource.Get();
    }

    const BufferDescription& Buffer::Description() const noexcept
    {
        return m_description;
    }
}
