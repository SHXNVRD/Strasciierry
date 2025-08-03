using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace Strasciierry.Core.Helpers;

public static class EnumHelper
{
    public static bool IsValidFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
        => !decimal.TryParse(flag.ToString(), out _);

    public static TFlag ParseFlags<TFlag>(string value, char separator) where TFlag : struct, Enum 
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        var flags = value.Replace(separator, ',');

        return Enum.Parse<TFlag>(flags);
    }
}
