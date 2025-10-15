using System.Runtime.InteropServices;

namespace Strasciierry.WinApi.User32;

public static partial class User32
{
    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr SetWindowLongW(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (Environment.Is64BitOperatingSystem)
            return SetWindowLongPtrW(hWnd, nIndex, dwNewLong);
        else
            return SetWindowLongW(hWnd, nIndex, dwNewLong);
    }
}
