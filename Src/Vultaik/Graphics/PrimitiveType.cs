// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// Defines how vertex data is ordered.
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// No documentation.
        /// </summary>
        Undefined = unchecked((int)0),

        /// <summary>
        /// No documentation.
        /// </summary>
        PointList = unchecked((int)1),

        /// <summary>
        /// The data is ordered as a sequence of line segments; each line segment is described by two new vertices. The count may be any positive integer.
        /// </summary>
        LineList = unchecked((int)2),

        /// <summary>
        /// The data is ordered as a sequence of line segments; each line segment is described by one new vertex and the last vertex from the previous line seqment. The count may be any positive integer.
        /// </summary>
        LineStrip = unchecked((int)3),

        /// <summary>
        /// The data is ordered as a sequence of triangles; each triangle is described by three new vertices. Back-face culling is affected by the current winding-order render state.
        /// </summary>
        TriangleList = unchecked((int)4),

        /// <summary>
        /// The data is ordered as a sequence of triangles; each triangle is described by two new vertices and one vertex from the previous triangle. The back-face culling flag is flipped automatically on even-numbered
        /// </summary>
        TriangleStrip = unchecked((int)5),

        /// <summary>
        /// No documentation.
        /// </summary>
        LineListWithAdjacency = unchecked((int)10),

        /// <summary>
        /// No documentation.
        /// </summary>
        LineStripWithAdjacency = unchecked((int)11),

        /// <summary>
        /// No documentation.
        /// </summary>
        TriangleListWithAdjacency = unchecked((int)12),

        /// <summary>
        /// No documentation.
        /// </summary>
        TriangleStripWithAdjacency = unchecked((int)13),

        /// <summary>
        /// No documentation.
        /// </summary>
        PatchList = unchecked((int)14),
    }
}
