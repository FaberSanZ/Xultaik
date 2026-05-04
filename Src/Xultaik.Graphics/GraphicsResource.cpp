#include "GraphicsResource.h"

namespace Xultaik::Graphics
{
    GraphicsResource::GraphicsResource(GraphicsDevice* graphicsDevice) noexcept
        : graphicsDevice_(graphicsDevice)
    {
    }

    GraphicsDevice* GraphicsResource::graphicsDevice() const noexcept
    {
        return graphicsDevice_;
    }
}
