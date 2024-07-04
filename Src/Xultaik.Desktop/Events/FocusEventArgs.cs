// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



namespace Xultaik.Desktop;

public readonly struct FocusEventArgs(bool value)
{
    public bool Focused => value;

    public static implicit operator bool(FocusEventArgs args) => args.Focused;
}
