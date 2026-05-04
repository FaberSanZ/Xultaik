#pragma once

#include "CommandListType.h"
#include "CommandQueue.h"
#include "DescriptorAllocator.h"
#include "GraphicsAdapter.h"
#include "RenderDescriptor.h"

#include <d3d12.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

#include <memory>

namespace Xultaik::Graphics
{
    class GraphicsDevice
    {
    public:
        GraphicsDevice();
        explicit GraphicsDevice(const RenderDescriptor& renderDescriptor);

        ID3D12Device* NativeDevice() const noexcept;
        ID3D12CommandQueue* DirectQueue() const noexcept;

        const GraphicsAdapter& Adapter() const noexcept;
        const RenderDescriptor& RenderSettings() const noexcept;

    private:
        void Initialize();
        void CreateDevice();
        void CreateQueuesAndHeaps();

        GraphicsAdapter m_adapter;
        RenderDescriptor m_renderSettings{};
        Microsoft::WRL::ComPtr<ID3D12Device> m_nativeDevice;
        std::unique_ptr<CommandQueue> m_directCommandQueue;
        std::unique_ptr<DescriptorAllocator> m_rtvAllocator;
        std::unique_ptr<DescriptorAllocator> m_dsvAllocator;
        std::unique_ptr<DescriptorAllocator> m_srvAllocator;
    };
}
