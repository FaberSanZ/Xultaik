#pragma once

#include "BufferDescription.h"
#include "GraphicsResource.h"

#include <d3d12.h>
#include <wrl/client.h>

#include <cstdint>
#include <vector>

namespace Xultaik::Graphics
{
    class Buffer : public GraphicsResource
    {
    public:
        Buffer(GraphicsDevice* graphicsDevice, const BufferDescription& description);

        void SetData(const void* data, std::size_t sizeInBytes);
        ID3D12Resource* Native() const noexcept;
        const BufferDescription& Description() const noexcept;

    private:
        BufferDescription m_description;
        Microsoft::WRL::ComPtr<ID3D12Resource> m_resource;
    };
}
