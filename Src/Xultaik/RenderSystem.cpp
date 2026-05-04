#include "RenderSystem.h"

#include <stdexcept>

namespace Xultaik
{
    namespace
    {
        Graphics::BufferDescription make_upload_buffer()
        {
            Graphics::BufferDescription description{};
            description.sizeInBytes = 256;
            description.heapType = Graphics::HeapType::Upload;
            return description;
        }

        Graphics::TextureData make_placeholder_texture()
        {
            Graphics::TextureData data{};
            data.width = 1;
            data.height = 1;
            data.format = Graphics::PixelFormat::R8G8B8A8_UNorm;
            data.pixels = {0, 0, 0, 255};
            return data;
        }
    }

    RenderSystem::RenderSystem(const Desktop::Window& window, const Graphics::RenderDescriptor& descriptor)
        : m_graphicsDevice(descriptor),
          m_swapChain(&m_graphicsDevice, window.NativeHandle(), descriptor),
          m_directQueue(&m_graphicsDevice, Graphics::CommandListType::Direct),
          m_commandList(&m_graphicsDevice, Graphics::CommandListType::Direct),
          m_rtvAllocator(&m_graphicsDevice, D3D12_DESCRIPTOR_HEAP_TYPE_RTV),
          m_dsvAllocator(&m_graphicsDevice, D3D12_DESCRIPTOR_HEAP_TYPE_DSV),
          m_srvAllocator(&m_graphicsDevice, D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV),
          m_uploadBuffer(&m_graphicsDevice, make_upload_buffer()),
          m_placeholderTexture(&m_graphicsDevice, make_placeholder_texture()),
          m_pipelineState(&m_graphicsDevice, Graphics::PipelineStateDescription{}),
          m_fence(&m_graphicsDevice)
    {
        CreateRenderTargets();
    }

    void RenderSystem::BeginFrame()
    {
        m_commandList.Reset();
        ClearCurrentBackBuffer();
    }

    void RenderSystem::EndFrame()
    {
        m_commandList.Close();
        ID3D12CommandList* lists[] = {m_commandList.Native()};
        m_directQueue.Execute(lists, 1);

        m_fenceValue += 1;
        m_fence.Signal(m_directQueue.Native(), m_fenceValue);
        m_swapChain.Present(true);
        m_fence.Wait(m_fenceValue);
    }

    Graphics::GraphicsDevice& RenderSystem::GraphicsDevice() noexcept
    {
        return m_graphicsDevice;
    }

    const Graphics::GraphicsDevice& RenderSystem::GraphicsDevice() const noexcept
    {
        return m_graphicsDevice;
    }

    Graphics::SwapChain& RenderSystem::SwapChain() noexcept
    {
        return m_swapChain;
    }

    const Graphics::SwapChain& RenderSystem::SwapChain() const noexcept
    {
        return m_swapChain;
    }

    void RenderSystem::CreateRenderTargets()
    {
        for (std::uint32_t index = 0; index < Graphics::SwapChain::FrameCount; ++index)
        {
            m_rtvHandles[index] = m_rtvAllocator.Allocate();
            m_graphicsDevice.NativeDevice()->CreateRenderTargetView(m_swapChain.BackBuffer(index), nullptr, m_rtvHandles[index]);
        }
    }

    void RenderSystem::ClearCurrentBackBuffer()
    {
        const auto backBufferIndex = m_swapChain.CurrentBackBufferIndex();
        auto* backBuffer = m_swapChain.BackBuffer(backBufferIndex);
        const auto rtv = m_rtvHandles[backBufferIndex];

        D3D12_RESOURCE_BARRIER barrier{};
        barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
        barrier.Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE;
        barrier.Transition.pResource = backBuffer;
        barrier.Transition.StateBefore = D3D12_RESOURCE_STATE_PRESENT;
        barrier.Transition.StateAfter = D3D12_RESOURCE_STATE_RENDER_TARGET;
        barrier.Transition.Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES;

        m_commandList.Native()->ResourceBarrier(1, &barrier);
        m_commandList.Native()->OMSetRenderTargets(1, &rtv, FALSE, nullptr);

        const float clearColor[4] = {0.08f, 0.08f, 0.12f, 1.0f};
        m_commandList.Native()->ClearRenderTargetView(rtv, clearColor, 0, nullptr);

        barrier.Transition.StateBefore = D3D12_RESOURCE_STATE_RENDER_TARGET;
        barrier.Transition.StateAfter = D3D12_RESOURCE_STATE_PRESENT;
        m_commandList.Native()->ResourceBarrier(1, &barrier);
    }
}
