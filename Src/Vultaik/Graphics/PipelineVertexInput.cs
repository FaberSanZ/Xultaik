// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System.Collections.Generic;
using Vortice.Vulkan;

namespace Vultaik
{

    /// <summary>
    /// Specifying parameters of a newly created pipeline vertex input state.
    /// </summary>
    public class PipelineVertexInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineVertexInput"/> class.
        /// </summary>
        public PipelineVertexInput()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineVertexInput"/> class.
        /// </summary>
        /// <param name="vertexBindingDescriptions">
        /// An List of <see cref="VertexInputBinding"/> class.
        /// </param>
        /// <param name="vertexAttributeDescriptions">
        /// An List of <see cref="VertexInputAttribute"/> class.
        /// </param>
        public PipelineVertexInput(List<VertexInputBinding> vertexBindingDescriptions, List<VertexInputAttribute> vertexAttributeDescriptions)
        {
            VertexBindingDescriptions = vertexBindingDescriptions;
            VertexAttributeDescriptions = vertexAttributeDescriptions;
        }

        /// <summary>
        /// An List of <see cref="VertexInputBinding"/> class.
        /// </summary>
        public List<VertexInputBinding> VertexBindingDescriptions { get; set; } = new List<VertexInputBinding>();

        /// <summary>
        /// An List of <see cref="VertexInputAttribute"/> class.
        /// </summary>
        public List<VertexInputAttribute> VertexAttributeDescriptions { get; set; } = new List<VertexInputAttribute>();
    }
}
