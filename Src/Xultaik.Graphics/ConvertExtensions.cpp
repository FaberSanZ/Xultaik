#include "ConvertExtensions.h"

namespace Xultaik::Graphics
{
    D3D12_HEAP_TYPE ConvertExtensions::ToD3D12(HeapType type)
    {
        return static_cast<D3D12_HEAP_TYPE>(type);
    }

    DXGI_FORMAT ConvertExtensions::ToDxgi(PixelFormat format)
    {
        return static_cast<DXGI_FORMAT>(format);
    }
}
