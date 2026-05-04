#include "CommandQueue.h"

#include "GraphicsDevice.h"

#include <stdexcept>

namespace Xultaik::Graphics
{
    CommandQueue::CommandQueue(GraphicsDevice* graphicsDevice, CommandListType type)
        : m_graphicsDevice(graphicsDevice),
          m_type(type)
    {
        D3D12_COMMAND_QUEUE_DESC desc{};
        desc.Type = static_cast<D3D12_COMMAND_LIST_TYPE>(m_type);
        desc.Priority = D3D12_COMMAND_QUEUE_PRIORITY_NORMAL;
        desc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
        desc.NodeMask = 0;

        if (FAILED(m_graphicsDevice->NativeDevice()->CreateCommandQueue(&desc, IID_PPV_ARGS(&m_nativeQueue))))
        {
            throw std::runtime_error("CreateCommandQueue failed.");
        }
    }

    void CommandQueue::Execute(ID3D12CommandList* const* lists, std::uint32_t count)
    {
        m_nativeQueue->ExecuteCommandLists(count, lists);
    }

    ID3D12CommandQueue* CommandQueue::Native() const noexcept
    {
        return m_nativeQueue.Get();
    }

    CommandListType CommandQueue::Type() const noexcept
    {
        return m_type;
    }
}
