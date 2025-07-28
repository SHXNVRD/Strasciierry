using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Color = System.Drawing.Color;

namespace Strasciierry.UI.Converters;

class SystemDrawingColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Color color)
            throw new ArgumentException("Parameter value must be a System.Drawing.Color");

        var uiColor = Windows.UI.Color.FromArgb(
            color.A,
            color.R,
            color.G,
            color.B);

        return new SolidColorBrush(uiColor);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not SolidColorBrush brush)
            throw new ArgumentException("Parameter value must be a Microsoft.UI.Xaml.Media.SolidColorBrush");

        var color = Color.FromArgb(
            brush.Color.A,
            brush.Color.R,
            brush.Color.G,
            brush.Color.B);

        return color;
    }
}
