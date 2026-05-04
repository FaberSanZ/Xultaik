#pragma once

#include "CommandListType.h"

#include <d3d12.h>
#include <wrl/client.h>

#include <cstdint>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class CommandQueue
    {
    public:
        CommandQueue(GraphicsDevice* graphicsDevice, CommandListType type);

        void Execute(ID3D12CommandList* const* lists, std::uint32_t count);
        ID3D12CommandQueue* Native() const noexcept;
        CommandListType Type() const noexcept;

    private:
        GraphicsDevice* m_graphicsDevice = nullptr;
        CommandListType m_type;
        Microsoft::WRL::ComPtr<ID3D12CommandQueue> m_nativeQueue;
    };
}
