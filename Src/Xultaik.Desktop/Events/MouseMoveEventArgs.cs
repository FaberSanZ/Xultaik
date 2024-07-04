// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System.Drawing;

namespace Xultaik.Desktop;
public readonly struct MouseMoveEventArgs(Point pos, Point previousPos)
{
    public Point Position => pos;

    public int X => Position.X;

    public int Y => Position.Y;

    public Point PreviousPosition => previousPos;

    public static implicit operator Point(MouseMoveEventArgs args) => args.Position;
}
