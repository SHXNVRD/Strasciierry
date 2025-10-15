using System.Runtime.InteropServices;

namespace Strasciierry.WinApi.Kernel32;

public partial class Kernel32
{
    [LibraryImport("kernel32.dll", SetLastError = false, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int GetCurrentPackageFullName(ref uint packageFullNameLength, byte[]? packageFullName);
}
