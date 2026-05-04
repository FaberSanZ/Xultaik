#include "GraphicsDevice.h"

#include "DescriptorAllocator.h"
#include "CommandQueue.h"

#include <iostream>
#include <stdexcept>

namespace Xultaik::Graphics
{
    namespace
    {
        void throw_if_failed(HRESULT hr, const char* message)
        {
            if (FAILED(hr))
            {
                throw std::runtime_error(message);
            }
        }
    }

    GraphicsDevice::GraphicsDevice()
    {
        Initialize();
    }

    GraphicsDevice::GraphicsDevice(const RenderDescriptor& renderDescriptor)
        : m_renderSettings(renderDescriptor)
    {
        Initialize();
    }

    ID3D12Device* GraphicsDevice::NativeDevice() const noexcept
    {
        return m_nativeDevice.Get();
    }

    ID3D12CommandQueue* GraphicsDevice::DirectQueue() const noexcept
    {
        return m_directCommandQueue ? m_directCommandQueue->Native() : nullptr;
    }

    const GraphicsAdapter& GraphicsDevice::Adapter() const noexcept
    {
        return m_adapter;
    }

    const RenderDescriptor& GraphicsDevice::RenderSettings() const noexcept
    {
        return m_renderSettings;
    }

    void GraphicsDevice::Initialize()
    {
        CreateDevice();
        CreateQueuesAndHeaps();
    }

    void GraphicsDevice::CreateDevice()
    {
        throw_if_failed(
            D3D12CreateDevice(m_adapter.Adapters().front().Get(), D3D_FEATURE_LEVEL_12_1, IID_PPV_ARGS(&m_nativeDevice)),
            "D3D12CreateDevice failed."
        );
    }

    void GraphicsDevice::CreateQueuesAndHeaps()
    {
        m_directCommandQueue = std::make_unique<CommandQueue>(this, CommandListType::Direct);
        m_rtvAllocator = std::make_unique<DescriptorAllocator>(this, D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
        m_dsvAllocator = std::make_unique<DescriptorAllocator>(this, D3D12_DESCRIPTOR_HEAP_TYPE_DSV);
        m_srvAllocator = std::make_unique<DescriptorAllocator>(this, D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV);
    }
}
