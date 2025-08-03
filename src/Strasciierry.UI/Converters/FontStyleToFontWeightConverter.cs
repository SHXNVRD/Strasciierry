using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Data;
using Strasciierry.Core.Helpers;
using Windows.UI.Text;
using FontStyle = System.Drawing.FontStyle;

namespace Strasciierry.UI.Converters;
internal class FontStyleToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not FontStyle fontStyle)
            throw new ArgumentException($"Parameter value must be a {typeof(FontStyle)}");
        if (!EnumHelper.IsValidFlag(fontStyle))
            throw new ArgumentException($"Invalid flags", nameof(value));

        return fontStyle.HasFlag(FontStyle.Bold)
            ? FontWeights.Bold
            : FontWeights.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not FontWeight fontWeight)
            throw new ArgumentException($"Parameter value must be a {typeof(FontWeight)}");

        return fontWeight.Weight switch
        {
            400 => FontStyle.Regular,
            700 => FontStyle.Bold,
            _ => throw new ArgumentException($"Cannot convert value into FontStyle")
        };
    }
}
