#pragma once

#include "PixelFormat.h"

#include <cstdint>
#include <vector>

namespace Xultaik::Graphics
{
    struct TextureData
    {
        std::uint32_t width = 0;
        std::uint32_t height = 0;
        PixelFormat format = PixelFormat::Unknown;
        std::vector<std::uint8_t> pixels;
    };
}
