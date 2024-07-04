// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



namespace Xultaik.Desktop;

public readonly struct MouseButtonEventArgs(int scanCode, InputAction action)
{
    public MouseButton Button => (MouseButton)scanCode;

    public int ScanCode => scanCode;

    public InputAction Action => action;

    public static implicit operator MouseButton(MouseButtonEventArgs e) => e.Button;
}
