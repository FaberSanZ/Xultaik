// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	StandardCursor.cs
=============================================================================*/

namespace Vultaik.Desktop
{
    /// <summary>
    /// Standard cursors.
    /// </summary>
    /// <remarks>
    /// Not every backend supports every standard cursor. Check availability with
    /// <see cref="Cursor.IsSupported(StandardCursor)"/> before changing to a standard cursor.
    /// </remarks>
    public enum StandardCursor
    {
        /// <summary>
        /// Default cursor.
        /// </summary>
        Default,

        /// <summary>
        /// Regular arrow cursor.
        /// </summary>
        Arrow,

        /// <summary>
        /// Text input I-beam cursor.
        /// </summary>
        IBeam,

        /// <summary>
        /// Crosshair cursor.
        /// </summary>
        Crosshair,

        /// <summary>
        /// Hand cursor.
        /// </summary>
        Hand,

        /// <summary>
        /// Horizontal resize arrow cursor.
        /// </summary>
        HResize,

        /// <summary>
        /// Vertical resize arrow cursor.
        /// </summary>
        VResize
    }
}
