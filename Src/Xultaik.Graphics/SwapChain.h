#pragma once

#include "RenderDescriptor.h"

#include <array>
#include <cstdint>
#include <d3d12.h>
#include <dxgi1_6.h>
#include <wrl/client.h>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class SwapChain
    {
    public:
        static constexpr std::uint32_t FrameCount = 2;

        SwapChain(GraphicsDevice* graphicsDevice, HWND windowHandle, const RenderDescriptor& descriptor);

        void Present(bool vsync);
        std::uint32_t CurrentBackBufferIndex() const noexcept;
        ID3D12Resource* BackBuffer(std::uint32_t index) const noexcept;
        IDXGISwapChain4* Native() const noexcept;

    private:
        Microsoft::WRL::ComPtr<IDXGISwapChain4> m_nativeSwapChain;
        std::array<Microsoft::WRL::ComPtr<ID3D12Resource>, FrameCount> m_backBuffers{};
    };
}
