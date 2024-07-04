// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System.Drawing;

namespace Xultaik.Desktop;

public readonly struct MouseWheelEventArgs(Point wheel)
{
    public Point Wheel => wheel;

    public int X => wheel.X;

    public int Y => wheel.Y;

    public static implicit operator Point(MouseWheelEventArgs e) => e.Wheel;
}
