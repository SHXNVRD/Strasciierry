using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Strasciierry.UI.Converters;

class StringToFontFamilyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string fontName && !string.IsNullOrEmpty(fontName))
            return new FontFamily(fontName);

        throw new ArgumentException("Parameter value must be a font name");
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is FontFamily fontFamily)
            return fontFamily.Source;

        throw new ArgumentException("Parameter value must be a FontFamily");
    }
}
