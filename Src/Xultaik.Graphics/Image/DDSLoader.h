#pragma once

#include "../TextureData.h"

#include <string_view>

namespace Xultaik::Graphics::Image
{
    struct DDSLoader
    {
        static TextureData Load(std::wstring_view path);
    };
}
