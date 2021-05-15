// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	IInputDevice.cs
=============================================================================*/

namespace Vultaik.Desktop
{
    /// <summary>
    /// Generic interface representing an input device.
    /// </summary>
    public interface IInputDevice
    {
        /// <summary>
        /// The name of this device, as reported by the hardware.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The index of this device.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Whether or not this device is currently connected.
        /// </summary>
        bool IsConnected { get; }
    }
}
