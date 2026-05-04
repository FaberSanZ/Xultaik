#include "Fence.h"

#include "GraphicsDevice.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    Fence::Fence(GraphicsDevice* graphicsDevice)
        : m_graphicsDevice(graphicsDevice)
    {
        if (FAILED(m_graphicsDevice->NativeDevice()->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(&m_fence))))
        {
            throw std::runtime_error("CreateFence failed.");
        }

        m_eventHandle = CreateEventW(nullptr, FALSE, FALSE, nullptr);
        if (m_eventHandle == nullptr)
        {
            throw std::runtime_error("CreateEventW failed.");
        }
    }

    Fence::~Fence()
    {
        if (m_eventHandle != nullptr)
        {
            CloseHandle(m_eventHandle);
        }
    }

    void Fence::Signal(ID3D12CommandQueue* queue, std::uint64_t value)
    {
        queue->Signal(m_fence.Get(), value);
    }

    void Fence::Wait(std::uint64_t value)
    {
        if (m_fence->GetCompletedValue() < value)
        {
            m_fence->SetEventOnCompletion(value, m_eventHandle);
            WaitForSingleObject(m_eventHandle, INFINITE);
        }
    }
}
