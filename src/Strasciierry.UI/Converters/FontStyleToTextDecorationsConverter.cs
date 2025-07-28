using Microsoft.UI.Xaml.Data;
using Strasciierry.Core.Helpers;
using Windows.UI.Text;
using FontStyle = System.Drawing.FontStyle;

namespace Strasciierry.UI.Converters;
internal class FontStyleToTextDecorationsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not FontStyle fontStyle)
            throw new ArgumentException($"Value must be of type {typeof(FontStyle)}");
        if (!EnumHelper.IsValidFlag(fontStyle))
            throw new ArgumentException($"Invalid flags", nameof(value));

        var result = TextDecorations.None;

        if ((fontStyle & FontStyle.Underline) != 0)
            result |= TextDecorations.Underline;
        if ((fontStyle & FontStyle.Strikeout) != 0)
            result |= TextDecorations.Strikethrough;

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not TextDecorations textDecorations)
            throw new ArgumentException($"Value must be of type {typeof(TextDecorations)}");
        if (!EnumHelper.IsValidFlag(textDecorations))
            throw new ArgumentException($"Invalid flags", nameof(value));

        var result = FontStyle.Regular;

        if ((textDecorations & TextDecorations.Underline) != 0)
            result |= FontStyle.Underline;
        if ((textDecorations & TextDecorations.Strikethrough) != 0)
            result |= FontStyle.Strikeout;

        return result;
    }
}
