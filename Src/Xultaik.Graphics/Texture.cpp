#include "Texture.h"

namespace Xultaik::Graphics
{
    Texture::Texture(GraphicsDevice* graphicsDevice, const TextureData& data)
        : GraphicsResource(graphicsDevice),
          m_data(data)
    {
    }

    ID3D12Resource* Texture::Native() const noexcept
    {
        return m_resource.Get();
    }
}
