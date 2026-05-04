#pragma once

#include <cstddef>

namespace Xultaik::Graphics
{
    struct ShaderByteCode
    {
        const void* data = nullptr;
        std::size_t size = 0;
    };
}
