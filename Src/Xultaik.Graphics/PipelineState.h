#pragma once

#include "PipelineStateDescription.h"

#include <d3d12.h>
#include <wrl/client.h>

namespace Xultaik::Graphics
{
    class GraphicsDevice;

    class PipelineState
    {
    public:
        PipelineState(GraphicsDevice* graphicsDevice, const PipelineStateDescription& description);

        ID3D12PipelineState* Native() const noexcept;

    private:
        Microsoft::WRL::ComPtr<ID3D12PipelineState> m_nativeState;
    };
}
