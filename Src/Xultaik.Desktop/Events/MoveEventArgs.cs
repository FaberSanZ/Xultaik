// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System.Drawing;

namespace Xultaik.Desktop;

public readonly struct MoveEventArgs(Point pos)
{
    public Point Position => pos;

    public int X => Position.X;

    public int Y => Position.Y;

    public static implicit operator Point(MoveEventArgs args) => args.Position;
}
