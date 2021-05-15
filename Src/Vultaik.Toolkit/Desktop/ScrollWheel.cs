// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ScrollWheel.cs
=============================================================================*/


namespace Vultaik.Desktop
{
    /// <summary>
    /// Represents a scroll wheel.
    /// </summary>
    public struct ScrollWheel
    {
        /// <summary>
        /// The X position of the scroll wheel.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// The Y position of the scroll wheel.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Creates a new instance of the scroll wheel struct.
        /// </summary>
        /// <param name="x">The X position of the scroll wheel.</param>
        /// <param name="y">The Y position of the scroll wheel.</param>
        public ScrollWheel(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
