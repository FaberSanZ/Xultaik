// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	Trigger.cs
=============================================================================*/


namespace Vultaik.Desktop
{
    /// <summary>
    /// Represents a trigger.
    /// </summary>
    public struct Trigger
    {
        /// <summary>
        /// The index of this trigger.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The position of this trigger; how far down it's currently pressed.
        /// </summary>
        public float Position { get; }

        /// <summary>
        /// Creates a new instance of the Trigger struct.
        /// </summary>
        /// <param name="index">The index of this trigger.</param>
        /// <param name="position">The position of this trigger.</param>
        public Trigger(int index, float position)
        {
            Index = index;
            Position = position;
        }
    }
}
