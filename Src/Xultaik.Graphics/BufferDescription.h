#pragma once

#include "BufferFlags.h"
#include "HeapType.h"

#include <cstdint>

namespace Xultaik::Graphics
{
    struct BufferDescription
    {
        std::uint32_t sizeInBytes = 0;
        std::uint32_t structureByteStride = 0;
        HeapType heapType = HeapType::Default;
        BufferFlags flags = BufferFlags::None;
    };
}
