// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System.Diagnostics.CodeAnalysis;

namespace Xultaik.Desktop;

public readonly struct WindowResourcePtr(IntPtr value) : IEquatable<WindowResourcePtr>
{
    public readonly IntPtr Value = value;

    public bool IsNull => Value == default;

    public static WindowResourcePtr Null => default;

    public static implicit operator IntPtr(WindowResourcePtr value) => value.Value;

    public static explicit operator WindowResourcePtr(IntPtr value) => new WindowResourcePtr(value);

    public static bool operator ==(WindowResourcePtr left, WindowResourcePtr right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(WindowResourcePtr left, WindowResourcePtr right)
    {
        return !left.Equals(right);
    }

    public bool Equals(WindowResourcePtr other)
    {
        return this.Value == other.Value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is WindowResourcePtr other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"0x{this.Value:x}";
    }

    public static WindowResourcePtr LoadIcon(string icoFilePath)
    {
        var img = Windows.Win32.PInvoke.LoadImage(null, icoFilePath, Windows.Win32.UI.WindowsAndMessaging.GDI_IMAGE_TYPE.IMAGE_ICON, 64, 64, Windows.Win32.UI.WindowsAndMessaging.IMAGE_FLAGS.LR_LOADFROMFILE);
        img.SetHandleAsInvalid();
        return new WindowResourcePtr(img.DangerousGetHandle());
    }
}
