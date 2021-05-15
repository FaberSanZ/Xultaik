// This file is part of Silk.NET.
// 
// You may modify and distribute Silk.NET under the terms
// of the MIT license. See the LICENSE file for details.

using System;

namespace Vultaik.Desktop
{
    /// <summary>
    /// Contains handy extensions for fetching buttons on gamepads.
    /// </summary>
    public static class GamepadExtensions
    {
        private static Button First(Gamepad gamepad, ButtonName name) // now without the LINQ
        {
            foreach (var button in gamepad.Buttons)
            {
                if (button.Name == name)
                {
                    return button;
                }
            }

            throw new PlatformNotSupportedException("This button is not supported by the input backend.");
        }

        /// <summary>
        /// Gets the <see cref="ButtonName.A" /> button.
        /// </summary>
        public static Button A(this Gamepad gamepad) => First(gamepad, ButtonName.A);

        /// <summary>
        /// Gets the <see cref="ButtonName.B" /> button.
        /// </summary>
        public static Button B(this Gamepad gamepad) => First(gamepad, ButtonName.B);

        /// <summary>
        /// Gets the <see cref="ButtonName.X" /> button.
        /// </summary>
        public static Button X(this Gamepad gamepad) => First(gamepad, ButtonName.X);

        /// <summary>
        /// Gets the <see cref="ButtonName.Y" /> button.
        /// </summary>
        public static Button Y(this Gamepad gamepad) => First(gamepad, ButtonName.Y);

        /// <summary>
        /// Gets the <see cref="ButtonName.LeftBumper" /> button.
        /// </summary>
        public static Button LeftBumper(this Gamepad gamepad) => First(gamepad, ButtonName.LeftBumper);

        /// <summary>
        /// Gets the <see cref="ButtonName.RightBumper" /> button.
        /// </summary>
        public static Button RightBumper(this Gamepad gamepad) => First(gamepad, ButtonName.RightBumper);

        /// <summary>
        /// Gets the <see cref="ButtonName.Back" /> button.
        /// </summary>
        public static Button Back(this Gamepad gamepad) => First(gamepad, ButtonName.Back);

        /// <summary>
        /// Gets the <see cref="ButtonName.Start" /> button.
        /// </summary>
        public static Button Start(this Gamepad gamepad) => First(gamepad, ButtonName.Start);

        /// <summary>
        /// Gets the <see cref="ButtonName.Home" /> button.
        /// </summary>
        public static Button Home(this Gamepad gamepad) => First(gamepad, ButtonName.Home);

        /// <summary>
        /// Gets the <see cref="ButtonName.LeftStick" /> button.
        /// </summary>
        public static Button LeftStick(this Gamepad gamepad) => First(gamepad, ButtonName.LeftStick);

        /// <summary>
        /// Gets the <see cref="ButtonName.RightStick" /> button.
        /// </summary>
        public static Button RightStick(this Gamepad gamepad) => First(gamepad, ButtonName.RightStick);

        /// <summary>
        /// Gets the <see cref="ButtonName.DPadUp" /> button.
        /// </summary>
        public static Button DPadUp(this Gamepad gamepad) => First(gamepad, ButtonName.DPadUp);

        /// <summary>
        /// Gets the <see cref="ButtonName.DPadRight" /> button.
        /// </summary>
        public static Button DPadRight(this Gamepad gamepad) => First(gamepad, ButtonName.DPadRight);

        /// <summary>
        /// Gets the <see cref="ButtonName.DPadDown" /> button.
        /// </summary>
        public static Button DPadDown(this Gamepad gamepad) => First(gamepad, ButtonName.DPadDown);

        /// <summary>
        /// Gets the <see cref="ButtonName.DPadLeft" /> button.
        /// </summary>
        public static Button DPadLeft(this Gamepad gamepad) => First(gamepad, ButtonName.DPadLeft);
    }
}
