#include "SwapChain.h"

#include "ConvertExtensions.h"
#include "GraphicsDevice.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    SwapChain::SwapChain(GraphicsDevice* graphicsDevice, HWND windowHandle, const RenderDescriptor& descriptor)
    {
        Microsoft::WRL::ComPtr<IDXGIFactory6> factory;
        if (FAILED(CreateDXGIFactory2(0, IID_PPV_ARGS(&factory))))
        {
            throw std::runtime_error("CreateDXGIFactory2 failed.");
        }

        DXGI_SWAP_CHAIN_DESC1 swapChainDesc{};
        swapChainDesc.Width = descriptor.width;
        swapChainDesc.Height = descriptor.height;
        swapChainDesc.Format = ConvertExtensions::ToDxgi(descriptor.format);
        swapChainDesc.SampleDesc.Count = 1;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.BufferCount = FrameCount;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swapChainDesc.AlphaMode = DXGI_ALPHA_MODE_UNSPECIFIED;
        swapChainDesc.Scaling = DXGI_SCALING_STRETCH;
        swapChainDesc.Flags = descriptor.vsync ? 0u : DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING;

        Microsoft::WRL::ComPtr<IDXGISwapChain1> swapChain1;
        if (FAILED(factory->CreateSwapChainForHwnd(
                graphicsDevice->DirectQueue(),
                windowHandle,
                &swapChainDesc,
                nullptr,
                nullptr,
                &swapChain1)))
        {
            throw std::runtime_error("CreateSwapChainForHwnd failed.");
        }

        if (FAILED(swapChain1.As(&m_nativeSwapChain)))
        {
            throw std::runtime_error("SwapChain cast failed.");
        }

        factory->MakeWindowAssociation(windowHandle, DXGI_MWA_NO_ALT_ENTER);

        for (std::uint32_t index = 0; index < FrameCount; ++index)
        {
            if (FAILED(m_nativeSwapChain->GetBuffer(index, IID_PPV_ARGS(m_backBuffers[index].ReleaseAndGetAddressOf()))))
            {
                throw std::runtime_error("SwapChain GetBuffer failed.");
            }
        }
    }

    void SwapChain::Present(bool vsync)
    {
        m_nativeSwapChain->Present(vsync ? 1u : 0u, 0);
    }

    std::uint32_t SwapChain::CurrentBackBufferIndex() const noexcept
    {
        return m_nativeSwapChain->GetCurrentBackBufferIndex();
    }

    ID3D12Resource* SwapChain::BackBuffer(std::uint32_t index) const noexcept
    {
        return m_backBuffers[index].Get();
    }

    IDXGISwapChain4* SwapChain::Native() const noexcept
    {
        return m_nativeSwapChain.Get();
    }
}
