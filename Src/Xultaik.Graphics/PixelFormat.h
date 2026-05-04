#pragma once

#include <dxgiformat.h>

namespace Xultaik::Graphics
{
    enum class PixelFormat : int
    {
        Unknown = DXGI_FORMAT_UNKNOWN,
        R8G8B8A8_UNorm = DXGI_FORMAT_R8G8B8A8_UNORM,
        R8G8B8A8_UNorm_SRGB = DXGI_FORMAT_R8G8B8A8_UNORM_SRGB,
        D32_Float = DXGI_FORMAT_D32_FLOAT
    };
}
