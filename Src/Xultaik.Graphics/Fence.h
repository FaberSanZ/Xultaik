#pragma once

#include <d3d12.h>
#include <wrl/client.h>

#include <cstdint>
#include <windows.h>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class Fence
    {
    public:
        explicit Fence(GraphicsDevice* graphicsDevice);
        ~Fence();

        void Signal(ID3D12CommandQueue* queue, std::uint64_t value);
        void Wait(std::uint64_t value);

    private:
        GraphicsDevice* m_graphicsDevice = nullptr;
        Microsoft::WRL::ComPtr<ID3D12Fence> m_fence;
        HANDLE m_eventHandle = nullptr;
    };
}
