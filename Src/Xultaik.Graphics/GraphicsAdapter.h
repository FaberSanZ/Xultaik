#pragma once

#include <dxgi1_6.h>
#include <wrl/client.h>

#include <vector>

namespace Xultaik::Graphics
{
    class GraphicsAdapter
    {
    public:
        GraphicsAdapter();

        const std::vector<Microsoft::WRL::ComPtr<IDXGIAdapter4>>& Adapters() const noexcept;

    private:
        void Enumerate();

        std::vector<Microsoft::WRL::ComPtr<IDXGIAdapter4>> m_adapters;
    };
}
