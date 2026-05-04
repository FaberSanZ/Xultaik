#pragma once

#include <d3d12.h>

namespace Xultaik::Graphics
{
    enum class CommandListType : int
    {
        Direct = D3D12_COMMAND_LIST_TYPE_DIRECT,
        Bundle = D3D12_COMMAND_LIST_TYPE_BUNDLE,
        Compute = D3D12_COMMAND_LIST_TYPE_COMPUTE,
        Copy = D3D12_COMMAND_LIST_TYPE_COPY,
        VideoDecode = D3D12_COMMAND_LIST_TYPE_VIDEO_DECODE,
        VideoProcess = D3D12_COMMAND_LIST_TYPE_VIDEO_PROCESS,
        VideoEncode = D3D12_COMMAND_LIST_TYPE_VIDEO_ENCODE
    };
}
