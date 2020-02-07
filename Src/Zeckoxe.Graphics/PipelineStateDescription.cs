// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineStateDescription.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class PipelineStateDescription
    {
        
        public InputAssemblyState InputAssemblyState { get; set; }

        public Framebuffer Framebuffer { get; set; }

        

        public ShaderBytecode Vertex { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationControl { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationEvaluation { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Geometry { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Fragment { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Compute { get; set; } = new ShaderBytecode(Array.Empty<byte>());

        //public ShaderBytecode AllGraphics { get; set; }

        //HLSL

        //public ShaderBytecode VertexShader { get; set; }
        //public ShaderBytecode PixelShader { get; set; }
        //public ShaderBytecode HullShader { get; set; }
        //public ShaderBytecode GeometryShader { get; set; }
        //public ShaderBytecode DomainShader { get; set; }
    }
}
