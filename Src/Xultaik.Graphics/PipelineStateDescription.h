#pragma once

#include "ShaderByteCode.h"

namespace Xultaik::Graphics
{
    struct PipelineStateDescription
    {
        ShaderByteCode vertexShader;
        ShaderByteCode pixelShader;
    };
}
