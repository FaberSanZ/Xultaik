// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Text;

namespace Xultaik.Graphics
{
    public class PipelineStateDescription
    {
        public ShaderByteCode VertexShader { get; set; }
        public ShaderByteCode PixelShader { get; set; } 
        public ShaderByteCode HullShader { get; set; }
        public ShaderByteCode GeometryShader { get; set; } 
        public ShaderByteCode DomainShader { get; set; }
    }
}
