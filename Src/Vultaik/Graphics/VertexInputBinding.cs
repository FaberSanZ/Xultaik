// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// Specifying vertex input binding.
    /// </summary>
    public class VertexInputBinding
    {
        public VertexInputBinding()
        {

        }



        /// <summary>
        /// Initializes a new instance of the <see cref="VertexInputBinding"/> class.
        /// </summary>
        /// <param name="binding">The binding number that this structure describes.</param>
        /// <param name="stride">
        /// The distance in bytes between two consecutive elements within the buffer.
        /// </param>
        /// <param name="inputRate">
        /// Specifies whether vertex attribute addressing is a function of the vertex index or of the
        /// instance index.
        /// </param>
        public VertexInputBinding(int binding, int stride, VertexInputRate inputRate)
        {
            Binding = binding;
            Stride = stride;
            InputRate = inputRate;
        }
        /// <summary>
        /// The binding number that this structure describes.
        /// </summary>
        public int Binding { get; set; }

        /// <summary>
        /// The distance in bytes between two consecutive elements within the buffer.
        /// </summary>
        public int Stride { get; set; }


        /// <summary>
        /// Specifies whether vertex attribute addressing is a function of the vertex index or of the
        /// instance index.
        /// </summary>
        public VertexInputRate InputRate { get; set; }

    }
}
