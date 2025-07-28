namespace Strasciierry.Core.Helpers;

public static class EnumHelper
{
    public static bool IsValidFlag<TFlag>(TFlag flag) where TFlag : Enum
        => !decimal.TryParse(flag.ToString(), out _);
}
