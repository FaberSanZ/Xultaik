// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// <p>Determines the fill mode to use when rendering triangles.</p>
    /// </summary>
    /// <remarks>
    /// <p>This enumeration is part of a rasterization-state object description (see <strong><see cref="RasterizationState"/></strong>).</p>
    /// </remarks>
    public enum FillMode : int
    {
        /// <summary>
        /// <dd> <p>Draw lines connecting the vertices. Adjacent vertices are not drawn.</p> </dd>
        /// </summary>
        Wireframe = unchecked((int)2),

        /// <summary>
        /// <dd> <p>Fill the triangles formed by the vertices. Adjacent vertices are not drawn.</p> </dd>
        /// </summary>
        Solid = unchecked((int)3),
    }
}
