// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Hat.cs
=============================================================================*/

namespace Vultaik.Desktop
{
    /// <summary>
    /// Represents a joystick hat.
    /// </summary>
    public struct Hat
    {
        /// <summary>
        /// The index of this hat.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The position of this hat.
        /// </summary>
        public Position2D Position { get; }

        /// <summary>
        /// Creates a new instance of the Hat struct.
        /// </summary>
        /// <param name="index">The index of the hat.</param>
        /// <param name="position">The position of the hat.</param>
        public Hat(int index, Position2D position)
        {
            Index = index;
            Position = position;
        }
    }
}
