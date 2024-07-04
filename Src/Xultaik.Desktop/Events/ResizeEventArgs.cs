// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Drawing;

namespace Xultaik.Desktop;

public readonly struct ResizeEventArgs(Size size)
{
    public Size Size => size;

    public int Width => Size.Width;

    public int Height => Size.Height;

    public float AspectRatio => size.Width / (float)size.Height;

    public static implicit operator Size(ResizeEventArgs args) => args.Size;
}
