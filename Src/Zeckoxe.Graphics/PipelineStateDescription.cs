// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineStateDescription.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Zeckoxe.Graphics
{

    public class GLSLByteCode
    {
        public ShaderBytecode Vertex { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationControl { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationEvaluation { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Geometry { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Fragment { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Compute { get; set; } = new ShaderBytecode(Array.Empty<byte>());
    }
    public class PipelineStateDescription
    {
        public Framebuffer Framebuffer { get; set; }

        public InputAssemblyState InputAssemblyState { get; set; }
        public RasterizationState RasterizationState { get; set; } = new RasterizationState();


        public GLSLByteCode GLSLByteCode { get; set; }

        //GLSL and HLSL
        public ShaderBytecode Vertex { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationControl { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode TessellationEvaluation { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Geometry { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Fragment { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Compute { get; set; } = new ShaderBytecode(Array.Empty<byte>());


        //HLSL
        public ShaderBytecode Pixel { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Hull { get; set; } = new ShaderBytecode(Array.Empty<byte>());
        public ShaderBytecode Domain { get; set; } = new ShaderBytecode(Array.Empty<byte>());
    }
}
