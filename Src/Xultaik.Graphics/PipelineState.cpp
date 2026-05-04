#include "PipelineState.h"

#include "GraphicsDevice.h"

namespace Xultaik::Graphics
{
    PipelineState::PipelineState(GraphicsDevice* graphicsDevice, const PipelineStateDescription& description)
    {
        (void)graphicsDevice;
        (void)description;
    }

    ID3D12PipelineState* PipelineState::Native() const noexcept
    {
        return m_nativeState.Get();
    }
}
