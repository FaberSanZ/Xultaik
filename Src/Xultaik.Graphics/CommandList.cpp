#include "CommandList.h"

#include "GraphicsDevice.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    CommandList::CommandList(GraphicsDevice* graphicsDevice, CommandListType type)
        : m_graphicsDevice(graphicsDevice),
          m_type(type)
    {
        if (FAILED(m_graphicsDevice->NativeDevice()->CreateCommandAllocator(static_cast<D3D12_COMMAND_LIST_TYPE>(m_type), IID_PPV_ARGS(&m_allocator))))
        {
            throw std::runtime_error("CreateCommandAllocator failed.");
        }
        if (FAILED(m_graphicsDevice->NativeDevice()->CreateCommandList(0, static_cast<D3D12_COMMAND_LIST_TYPE>(m_type), m_allocator.Get(), nullptr, IID_PPV_ARGS(&m_list))))
        {
            throw std::runtime_error("CreateCommandList failed.");
        }
        m_list->Close();
    }

    void CommandList::Reset()
    {
        m_allocator->Reset();
        m_list->Reset(m_allocator.Get(), nullptr);
    }

    void CommandList::Close()
    {
        m_list->Close();
    }

    ID3D12GraphicsCommandList* CommandList::Native() const noexcept
    {
        return m_list.Get();
    }
}
