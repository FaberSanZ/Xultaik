#pragma once

#include <d3d12.h>

namespace Xultaik::Graphics
{
    enum class HeapType : int
    {
        Default = D3D12_HEAP_TYPE_DEFAULT,
        Upload = D3D12_HEAP_TYPE_UPLOAD,
        Readback = D3D12_HEAP_TYPE_READBACK
    };
}
