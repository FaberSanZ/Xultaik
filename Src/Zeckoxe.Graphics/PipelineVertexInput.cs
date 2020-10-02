// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

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
