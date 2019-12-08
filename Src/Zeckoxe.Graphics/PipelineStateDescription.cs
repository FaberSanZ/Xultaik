using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class PipelineStateDescription
    {
        public ShaderByteCode? VertexShader { get; set; } = null;
        public ShaderByteCode? PixelShader { get; set; } = null;
        public ShaderByteCode? HullShader { get; set; } = null;
        public ShaderByteCode? GeometryShader { get; set; } = null;
        public ShaderByteCode? DomainShader { get; set; } = null;
    }
}
