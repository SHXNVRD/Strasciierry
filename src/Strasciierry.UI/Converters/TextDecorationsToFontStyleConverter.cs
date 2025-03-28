using Microsoft.UI.Xaml.Data;
using Windows.UI.Text;
using FontStyle = System.Drawing.FontStyle;

namespace Strasciierry.UI.Converters;
internal class TextDecorationsToFontStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not FontStyle fontStyle)
            throw new ArgumentException($"Value must be of type {typeof(FontStyle)}");

        var allowedFlags = FontStyle.Underline | FontStyle.Strikeout;
        if ((fontStyle & ~allowedFlags) != 0)
            throw new ArgumentException("Invalid valur in FontStyle");

        return fontStyle switch
        {
            FontStyle.Regular => TextDecorations.None,
            FontStyle.Underline => TextDecorations.Underline,
            FontStyle.Strikeout => TextDecorations.Strikethrough,
            FontStyle.Underline | FontStyle.Strikeout => TextDecorations.Underline | TextDecorations.Strikethrough,
            _ => throw new ArgumentException($"Failed to convert value to {typeof(TextDecorations)}")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not TextDecorations textDecorations)
            throw new ArgumentException($"Value must be of type {typeof(TextDecorations)}");

        var allowedFlags = TextDecorations.Underline | TextDecorations.Strikethrough;
        if ((textDecorations & ~allowedFlags) != 0)
            throw new ArgumentException("Invalid flags in TextDecorations");

        return textDecorations switch
        {
            TextDecorations.None => FontStyle.Regular,
            TextDecorations.Underline => FontStyle.Underline,
            TextDecorations.Strikethrough => FontStyle.Strikeout,
            TextDecorations.Underline | TextDecorations.Strikethrough => FontStyle.Underline | FontStyle.Strikeout,
            _ => throw new ArgumentException($"Failed to convert value to {typeof(FontStyle)}")
        };
    }
}
