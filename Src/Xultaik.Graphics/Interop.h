#pragma once

#include <cstddef>
#include <cstdint>
#include <string>
#include <string_view>

namespace Xultaik::Graphics
{
    struct Interop
    {
        static void* Alloc(std::size_t bytes);
        static void Free(void* ptr) noexcept;
        static std::string ToUtf8(std::wstring_view value);
    };
}
