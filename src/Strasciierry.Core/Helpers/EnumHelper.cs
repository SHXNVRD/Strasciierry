using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Strasciierry.Core.Helpers;

public static class EnumHelper
{
    // https://learn.microsoft.com/en-us/visualstudio/profiling/performance-insights-enum-tostring?view=vs-2022
    private static readonly ConcurrentDictionary<Type, ulong> _validFlagsCache = new();

    public static bool IsValidFlag<TFlag>(TFlag flag) where TFlag : struct, Enum
    {
        var type = typeof(TFlag);
        if (!_validFlagsCache.TryGetValue(type, out var validMask))
        {
            validMask = CalculateValidMask<TFlag>();
            _validFlagsCache[type] = validMask;
        }

        var flagValue = Convert.ToUInt64(flag);
        return (flagValue & ~validMask) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong CalculateValidMask<TFlag>() where TFlag : struct, Enum
    {
        var mask = 0ul;
        foreach (var value in Enum.GetValues<TFlag>())
        {
            mask |= Convert.ToUInt64(value);
        }
        return mask;
    }

    public static TFlag ParseFlags<TFlag>(string value, char separator) where TFlag : struct, Enum 
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        var flags = value.Replace(separator, ',');

        return Enum.Parse<TFlag>(flags);
    }
}
