#pragma once

#include "CommandListType.h"

#include <d3d12.h>
#include <wrl/client.h>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class CommandList
    {
    public:
        CommandList(GraphicsDevice* graphicsDevice, CommandListType type);

        void Reset();
        void Close();
        ID3D12GraphicsCommandList* Native() const noexcept;

    private:
        GraphicsDevice* m_graphicsDevice = nullptr;
        CommandListType m_type;
        Microsoft::WRL::ComPtr<ID3D12CommandAllocator> m_allocator;
        Microsoft::WRL::ComPtr<ID3D12GraphicsCommandList> m_list;
    };
}
