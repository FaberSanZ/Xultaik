// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Collections;
using System.Numerics;

namespace Xultaik.Desktop;

public class KeyboardState
{
    private readonly BitArray _keys;
    private readonly BitArray _keysPrevious;

    public KeyboardState()
    {
        this._keys = new BitArray((int)Keys.Count);
        this._keysPrevious = new BitArray((int)Keys.Count);
    }

    public bool IsAnyKeyDown
    {
        get
        {
            for (var i = 0; i < _keys.Length; ++i)
            {
                if (_keys[i])
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool AltPressed { get; set; }

    public bool Shift => this.IsKeyDown(Keys.Shift);

    public bool Control => this.IsKeyDown(Keys.Control);

    public bool Enter => this.IsKeyDown(Keys.Enter);

    public bool LeftWindows => this.IsKeyDown(Keys.LeftWindows);

    public bool RightWindows => this.IsKeyDown(Keys.RightWindows);

    public bool this[Keys Keys]
    {
        set => _keys[(int)Keys] = value;
        get => _keys[(int)Keys];
    }
    public bool this[int index]
    {
        set => _keys[index] = value;
        get => _keys[index];
    }

    public bool IsKeyDown(Keys Key)
    {
        return _keys[(int)Key];
    }
    public bool IsKeyUp(Keys Key)
    {
        return !_keys[(int)Key];

    }
    public bool IsKeyPress(Keys Key)
    {
        if (_keysPrevious[(int)Key])
        {
            if (IsKeyDown(Key))
            {
                _keysPrevious[(int)Key] = false;
                return true;
            }
        }
        else if (IsKeyUp(Key))
        {
            _keysPrevious[(int)Key] = true;
        }

        return false;
    }
}
