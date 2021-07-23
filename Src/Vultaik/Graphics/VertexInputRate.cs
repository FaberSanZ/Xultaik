// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// Specify rate at which vertex attributes are pulled from buffers.
    /// </summary>
    public enum VertexInputRate
    {
        /// <summary>
        /// Specifies that vertex attribute addressing is a function of the vertex index.
        /// </summary>
        Vertex = 0,

        /// <summary>
        /// Specifies that vertex attribute addressing is a function of the instance index.
        /// </summary>
        Instance = 1
    }
}
