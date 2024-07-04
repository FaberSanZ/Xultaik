// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



namespace Xultaik.Desktop;

[Flags]
public enum KeyModifiers
{
    /// <summary>
    /// if one or more Shift keys were held down.
    /// </summary>
    Shift = 0x0001,

    /// <summary>
    /// If one or more Control keys were held down.
    /// </summary>
    Control = 0x0002,

    /// <summary>
    /// If one or more Alt keys were held down.
    /// </summary>
    Alt = 0x0004,

    /// <summary>
    /// If one or more Super keys were held down.
    /// </summary>
    Super = 0x0008,

    /// <summary>
    /// If caps lock is enabled.
    /// </summary>
    CapsLock = 0x0010,

    /// <summary>
    /// If num lock is enabled.
    /// </summary>
    NumLock = 0x0020,
}
