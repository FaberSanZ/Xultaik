#pragma once

#include <array>
#include <cstdint>

#include "../Xultaik.Desktop/Window.h"
#include "../Xultaik.Graphics/Buffer.h"
#include "../Xultaik.Graphics/BufferDescription.h"
#include "../Xultaik.Graphics/CommandList.h"
#include "../Xultaik.Graphics/CommandListType.h"
#include "../Xultaik.Graphics/CommandQueue.h"
#include "../Xultaik.Graphics/DescriptorAllocator.h"
#include "../Xultaik.Graphics/GraphicsDevice.h"
#include "../Xultaik.Graphics/Fence.h"
#include "../Xultaik.Graphics/HeapType.h"
#include "../Xultaik.Graphics/PipelineState.h"
#include "../Xultaik.Graphics/PipelineStateDescription.h"
#include "../Xultaik.Graphics/RenderDescriptor.h"
#include "../Xultaik.Graphics/SwapChain.h"
#include "../Xultaik.Graphics/Texture.h"
#include "../Xultaik.Graphics/TextureData.h"

namespace Xultaik
{
    class RenderSystem
    {
    public:
        RenderSystem(const Desktop::Window& window, const Graphics::RenderDescriptor& descriptor = {});

        void BeginFrame();
        void EndFrame();

        Graphics::GraphicsDevice& GraphicsDevice() noexcept;
        const Graphics::GraphicsDevice& GraphicsDevice() const noexcept;

        Graphics::SwapChain& SwapChain() noexcept;
        const Graphics::SwapChain& SwapChain() const noexcept;

    private:
        void CreateRenderTargets();
        void ClearCurrentBackBuffer();

        Graphics::GraphicsDevice m_graphicsDevice;
        Graphics::SwapChain m_swapChain;
        Graphics::CommandQueue m_directQueue;
        Graphics::CommandList m_commandList;
        Graphics::DescriptorAllocator m_rtvAllocator;
        Graphics::DescriptorAllocator m_dsvAllocator;
        Graphics::DescriptorAllocator m_srvAllocator;
        Graphics::Buffer m_uploadBuffer;
        Graphics::Texture m_placeholderTexture;
        Graphics::PipelineState m_pipelineState;
        Graphics::Fence m_fence;
        std::uint64_t m_fenceValue = 0;
        std::array<D3D12_CPU_DESCRIPTOR_HANDLE, Graphics::SwapChain::FrameCount> m_rtvHandles{};
    };
}
