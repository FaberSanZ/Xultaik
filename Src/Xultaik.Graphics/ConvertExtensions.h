#pragma once

#include "HeapType.h"
#include "IndexType.h"
#include "PixelFormat.h"
#include "PrimitiveType.h"

#include <d3d12.h>

namespace Xultaik::Graphics
{
    struct ConvertExtensions
    {
        static D3D12_HEAP_TYPE ToD3D12(HeapType type);
        static DXGI_FORMAT ToDxgi(PixelFormat format);
    };
}
