using Microsoft.UI.Xaml.Data;
using Strasciierry.Core.Helpers;
using Windows.UI.Text;

namespace Strasciierry.UI.Converters;

class SystemDrawingFontStyleToWindowsUiFontStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not System.Drawing.FontStyle fontStyle)
            throw new ArgumentException($"Value must be of type {typeof(System.Drawing.FontStyle)}");
        if (!EnumHelper.IsValidFlag(fontStyle))
            throw new ArgumentException($"Invalid flags", nameof(value));

        return (fontStyle & System.Drawing.FontStyle.Italic) != 0
            ? FontStyle.Italic
            : FontStyle.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not FontStyle fontStyle)
            throw new ArgumentException($"Value must be of type {typeof(FontStyle)}");
        if (!EnumHelper.IsValidFlag(fontStyle))
            throw new ArgumentException($"Invalid flags", nameof(value));

        return (fontStyle & FontStyle.Italic) != 0
            ? System.Drawing.FontStyle.Italic
            : System.Drawing.FontStyle.Regular;
    }
}
