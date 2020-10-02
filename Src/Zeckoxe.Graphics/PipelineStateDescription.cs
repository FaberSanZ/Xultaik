// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

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
        public Framebuffer Framebuffer { get; set; }

        public InputAssemblyState InputAssemblyState { get; set; }

        public RasterizationState RasterizationState { get; set; } = new RasterizationState();

        public MultisampleState MultisampleState { get; set; } = new MultisampleState();

        public PipelineVertexInput PipelineVertexInput { get; set; }

        public List<ShaderBytecode> Shaders { get; set; } = new List<ShaderBytecode>();

        public List<DescriptorSetLayout> Layouts { get; set; } = new List<DescriptorSetLayout>();

        


    }
}
