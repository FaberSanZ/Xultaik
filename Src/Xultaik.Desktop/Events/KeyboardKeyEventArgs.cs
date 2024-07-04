// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


namespace Xultaik.Desktop;

public readonly struct KeyboardKeyEventArgs(int scanCode, InputAction action, bool altPress)
{
    public Keys Key => (Keys)scanCode;

    public int ScanCode => scanCode;

    public InputAction Action => action;

    public bool AltPressed => altPress;


    public static implicit operator Keys(KeyboardKeyEventArgs e) => e.Key;
}
