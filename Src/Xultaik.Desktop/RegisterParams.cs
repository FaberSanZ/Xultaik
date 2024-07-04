// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Windows.Win32;
using Windows.Win32.Foundation;

namespace Xultaik.Desktop;

internal readonly struct RegisterParams(FreeLibrarySafeHandle handler, string hashName)
{
    public FreeLibrarySafeHandle Handler => handler;

    public string HashName => hashName;

    public HINSTANCE HInstance => (HINSTANCE)Handler.DangerousGetHandle();
}
