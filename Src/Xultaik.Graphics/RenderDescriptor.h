#pragma once

#include "PixelFormat.h"

#include <cstdint>

namespace Xultaik::Graphics
{
    struct RenderDescriptor
    {
        std::uint32_t width = 1280;
        std::uint32_t height = 720;
        PixelFormat format = PixelFormat::R8G8B8A8_UNorm;
        bool vsync = true;
    };
}
