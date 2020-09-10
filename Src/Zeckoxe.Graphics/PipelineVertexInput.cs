// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	PipelineVertexInput.cs
=============================================================================*/



using System.Collections.Generic;

namespace Zeckoxe.Graphics
{
    public class PipelineVertexInput
    {
        public PipelineVertexInput()
        {

        }

        public List<VertexInputBinding> VertexBindingDescriptions { get; set; } = new List<VertexInputBinding>();
        public List<VertexInputAttribute> VertexAttributeDescriptions { get; set; } = new List<VertexInputAttribute>();
    }
}
