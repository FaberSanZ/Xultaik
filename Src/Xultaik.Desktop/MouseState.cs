// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Collections;
using System.Drawing;
using System.Numerics;

namespace Xultaik.Desktop;

public class MouseState
{
    private readonly BitArray _mouse;
    private readonly BitArray _mousePrevious;

    public MouseState()
    {
        this._mouse = new BitArray((int)MouseButton.Count);
        this._mousePrevious = new BitArray((int)MouseButton.Count);
    }

    public Point Position { get; set; }

    /// <summary>
    /// Gets the cursor position of the last frame.
    /// </summary>
    public Point PreviousPosition { get; set; }

    public Point Wheel { get; set; }

    public int X => Position.X;

    public int Y => Position.Y;

    public Point Delta => new (Position.X - PreviousPosition.X, Position.Y - PreviousPosition.Y);

    public bool IsAnyButtonDown
    {
        get
        {
            for (var i = 0; i < _mouse.Length; ++i)
            {
                if (_mouse[i])
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool this[MouseButton MouseButton]
    {
        set => _mouse[(int)MouseButton] = value;
        get => _mouse[(int)MouseButton];
    }
    public bool this[int index]
    {
        set => _mouse[index] = value;
        get => _mouse[index];
    }

    public void SetNewFrameData(Point position, Point wheel)
    {
        this.PreviousPosition = this.Position;
        this.Position = position;
        this.Wheel = wheel;
    }
    public bool IsButtonDown(MouseButton MouseButton)
    {
        return _mouse[(int)MouseButton];
    }
    public bool IsButtonUp(MouseButton MouseButton)
    {
        return !_mouse[(int)MouseButton];

    }
    public bool IsButtonPress(MouseButton MouseButton)
    {
        if (_mousePrevious[(int)MouseButton])
        {
            if (IsButtonDown(MouseButton))
            {
                _mousePrevious[(int)MouseButton] = false;
                return true;
            }
        }
        else if (IsButtonUp(MouseButton))
        {
            _mousePrevious[(int)MouseButton] = true;
        }

        return false;
    }
}
