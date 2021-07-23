// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    /// <summary>
    /// Interpret primitive front-facing orientation.
    /// </summary>
    public enum FrontFace
    {
        /// <summary>
        /// Specifies that a triangle with positive area is considered front-facing.
        /// </summary>
        CounterClockwise = 0,


        /// <summary>
        /// Specifies that a triangle with negative area is considered front-facing.
        /// </summary>
        Clockwise = 1
    }
}
