// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// Indicates triangles facing a particular direction are not drawn.
    /// </summary>
    /// <remarks>
    /// This enumeration is part of a rasterization-state object description (see <see cref="RasterizationState"/>). 
    /// </remarks>
    public enum CullMode
    {
        /// <summary>
        /// Always draw all triangles. 
        /// </summary>
        None = 1,

        /// <summary>
        /// Do not draw triangles that are front-facing. 
        /// </summary>
        Front = 2,

        /// <summary>
        /// Do not draw triangles that are back-facing. 
        /// </summary>
        Back = 3,
    }
}
