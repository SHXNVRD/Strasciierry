using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Color = System.Drawing.Color;

namespace Strasciierry.UI.Converters;

public class ColorToContrastSolidColorBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Color color)
            throw new ArgumentException("Value must be Windows.UI.Color", nameof(value));

        return (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) > 186
            ? new SolidColorBrush(Colors.Black)
            : new SolidColorBrush(Colors.White); 
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => throw new NotImplementedException();
}
