using System.Drawing;
using ABI.Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Strasciierry.Core.Helpers;
using Windows.Devices.AllJoyn;

namespace Strasciierry.UI.Converters;

internal class FontStyleToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is not string flagString)
            throw new ArgumentException("Parameter must be a flags enumeration", nameof(parameter));
        if (value is not FontStyle flagValue)
            throw new ArgumentException($"Value must be of type {typeof(FontStyle)}");
        if (!EnumHelper.IsValidFlag(flagValue))
            throw new ArgumentException($"Invalid flags", nameof(value));

        var parsingFlags = flagString.Split('|');
        var flags = FontStyle.Regular;

        foreach(var flag in parsingFlags)
        {
            flags |= Enum.Parse<FontStyle>(flag);
        }

        return (flags & flagValue) != 0 
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
