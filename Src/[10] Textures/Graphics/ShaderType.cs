using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public enum ShaderType
    {
        None = 0,

        VertexShader = 1,

        PixelShader = 2,

        HullShader = 3,

        GeometryShader = 4,

        DomainShader = 5,

        ComputeShader = 6
    }
}
