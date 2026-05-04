#include "GraphicsAdapter.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    GraphicsAdapter::GraphicsAdapter()
    {
        Enumerate();
    }

    const std::vector<Microsoft::WRL::ComPtr<IDXGIAdapter4>>& GraphicsAdapter::Adapters() const noexcept
    {
        return m_adapters;
    }

    void GraphicsAdapter::Enumerate()
    {
        Microsoft::WRL::ComPtr<IDXGIFactory6> factory;
        if (FAILED(CreateDXGIFactory2(0, IID_PPV_ARGS(&factory))))
        {
            throw std::runtime_error("CreateDXGIFactory2 failed.");
        }

        for (UINT index = 0;; ++index)
        {
            Microsoft::WRL::ComPtr<IDXGIAdapter1> adapter1;
            const HRESULT hr = factory->EnumAdapterByGpuPreference(
                index,
                DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
                IID_PPV_ARGS(&adapter1));
            if (hr == DXGI_ERROR_NOT_FOUND)
            {
                break;
            }
            if (FAILED(hr))
            {
                continue;
            }

            DXGI_ADAPTER_DESC1 desc{};
            adapter1->GetDesc1(&desc);
            if (desc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE)
            {
                continue;
            }

            Microsoft::WRL::ComPtr<IDXGIAdapter4> adapter4;
            if (SUCCEEDED(adapter1.As(&adapter4)))
            {
                m_adapters.push_back(adapter4);
            }
        }

        if (m_adapters.empty())
        {
            throw std::runtime_error("No hardware adapter found.");
        }
    }
}
