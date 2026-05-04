#include "Interop.h"

#include <Windows.h>

#include <cstdlib>

namespace Xultaik::Graphics
{
    void* Interop::Alloc(std::size_t bytes)
    {
        return bytes == 0 ? nullptr : std::malloc(bytes);
    }

    void Interop::Free(void* ptr) noexcept
    {
        std::free(ptr);
    }

    std::string Interop::ToUtf8(std::wstring_view value)
    {
        if (value.empty())
        {
            return {};
        }

        const int required = WideCharToMultiByte(CP_UTF8, 0, value.data(), static_cast<int>(value.size()), nullptr, 0, nullptr, nullptr);
        std::string result(static_cast<std::size_t>(required), '\0');
        WideCharToMultiByte(CP_UTF8, 0, value.data(), static_cast<int>(value.size()), result.data(), required, nullptr, nullptr);
        return result;
    }
}
