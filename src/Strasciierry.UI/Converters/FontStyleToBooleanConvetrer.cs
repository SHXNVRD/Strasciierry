using System.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Strasciierry.Core.Helpers;

namespace Strasciierry.UI.Converters;

internal class FontStyleToBooleanConvetrer : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is not string flagString)
            throw new ArgumentException("Parameter must be a flags enumeration", nameof(parameter));
        if (value is not FontStyle flagValue)
            throw new ArgumentException($"Value must be of type {typeof(FontStyle)}");
        if (!EnumHelper.IsValidFlag(flagValue))
            throw new ArgumentException($"Invalid flags", nameof(value));

        var flags = EnumHelper.ParseFlags<FontStyle>(flagString, '|');

        return (flags & flagValue) != 0;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (parameter is not string flagString)
            throw new ArgumentException("Parameter must be an flag name", nameof(parameter));

        return EnumHelper.ParseFlags<FontStyle>(flagString, '|');
    }
}
