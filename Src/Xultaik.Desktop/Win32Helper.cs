// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Windows.Win32;
using Windows.Win32.Foundation;

namespace Xultaik.Desktop;
internal static class Win32Helper
{
    public static FreeLibrarySafeHandle GetDefaultModule() => PInvoke.GetModuleHandle((string)null!);
    public static string GenerateHash() => Guid.NewGuid().ToString().ToUpper();

    public const int MK_SHIFT = 0x0004;
    public static ushort GET_KEYSTATE_WPARAM(WPARAM wParam) => LOWORD(wParam);
    public static ushort GET_XBUTTON_WPARAM(WPARAM wParam) => HIWORD(wParam);
    public static int GET_X_LPARAM(LPARAM lParam) => (short)LOWORD((nuint)lParam.Value);
    public static int GET_Y_LPARAM(LPARAM lParam) => (short)HIWORD((nuint)lParam.Value);
    public static short GET_WHEEL_DELTA_WPARAM(WPARAM wPARAM) => (short)HIWORD(wPARAM);
    public static ushort HIWORD(nuint l) => (ushort)((l >> 16) & 0xFFFF);
    public static ushort LOWORD(nuint l) => (ushort)(l & 0xFFFF);

}
