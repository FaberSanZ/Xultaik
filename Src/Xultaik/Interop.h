#pragma once

#include <cstddef>
#include <cstdint>
#include <cstdlib>
#include <cstring>
#include <string>
#include <string_view>

namespace Xultaik
{
    struct Interop
    {
        static void* Alloc(std::size_t bytes);
        static void Free(void* ptr) noexcept;
        static std::string ToUtf8(std::wstring_view value);
    };
}
