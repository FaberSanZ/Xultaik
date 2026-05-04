#pragma once

#include "GraphicsResource.h"
#include "TextureData.h"

#include <d3d12.h>
#include <wrl/client.h>

namespace Xultaik::Graphics
{
    class Texture : public GraphicsResource
    {
    public:
        Texture(GraphicsDevice* graphicsDevice, const TextureData& data);

        ID3D12Resource* Native() const noexcept;

    private:
        TextureData m_data;
        Microsoft::WRL::ComPtr<ID3D12Resource> m_resource;
    };
}
