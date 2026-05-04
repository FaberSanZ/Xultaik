#pragma once

#include <d3d12.h>
#include <wrl/client.h>

#include <cstdint>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class DescriptorAllocator
    {
    public:
        DescriptorAllocator(GraphicsDevice* graphicsDevice, D3D12_DESCRIPTOR_HEAP_TYPE type, std::uint32_t descriptorCount = 64);

        D3D12_CPU_DESCRIPTOR_HANDLE Allocate(std::uint32_t count = 1);
        ID3D12DescriptorHeap* Heap() const noexcept;

    private:
        GraphicsDevice* m_graphicsDevice = nullptr;
        Microsoft::WRL::ComPtr<ID3D12DescriptorHeap> m_heap;
        UINT m_descriptorSize = 0;
        std::uint32_t m_nextIndex = 0;
    };
}
