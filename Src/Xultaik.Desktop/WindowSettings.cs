// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Drawing;

namespace Xultaik.Desktop;

public struct WindowSettings
{
    public static WindowSettings Default => new WindowSettings
    {
        Title = "Default",
        Size = new(600, 400),
        Position = new(0, 0),
        State = WindowState.Normal,
        Border = WindowBorder.Resizable,
        CursorMode = CursorMode.Normal,
        UpdateFrequency = null,
    };

    public string Title { get; set; }
    public Size Size { get; set; }
    public Point Position { get; set; }
    public WindowState State { get; set; }
    public WindowBorder Border { get; set; }
    public CursorMode CursorMode { get; set; }
    public double? UpdateFrequency { get; set; }
    public WindowResourcePtr Icon { get; set; }
}
