using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Data;
using Windows.UI.Text;
using FontStyle = System.Drawing.FontStyle;

namespace Strasciierry.UI.Converters;
internal class FontWeightToDrawingFontStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (!Enum.IsDefined(typeof(FontStyle), value))
            throw new ArgumentException($"Parameter value must be a {typeof(FontStyle)}");

        var enumValue = (FontStyle)value;

        return enumValue switch
        {
            FontStyle.Regular => FontWeights.Normal,
            FontStyle.Bold => FontWeights.Bold,
            _ => throw new ArgumentException($"Cannot convert value into FontWeight")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is FontWeight fontWeight)
        {
            return fontWeight.Weight switch
            {
                400 => FontStyle.Regular,
                700 => FontStyle.Bold,
                _ => throw new ArgumentException($"Cannot convert value into FontStyle")
            };
        }

        throw new ArgumentException($"Prameter value must be a {typeof(FontWeight)}");
    }
}
