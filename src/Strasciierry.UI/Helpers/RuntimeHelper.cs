using System.Reflection;
using System.Runtime.InteropServices;
using CommunityToolkit.WinUI;
using Strasciierry.WinApi.Kernel32;
using Windows.ApplicationModel;

namespace Strasciierry.UI.Helpers;

public class RuntimeHelper
{
    public static bool IsMSIX
    {
        get
        {
            var length = 0U;

            return Kernel32.GetCurrentPackageFullName(ref length, null) != 15700;
        }
    }

    public static string GetVersionDescription()
    {
        Version version;

        if (IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
