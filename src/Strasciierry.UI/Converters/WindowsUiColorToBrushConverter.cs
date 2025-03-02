using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Strasciierry.UI.Converters;

class WindowsUiColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Color color)
            return new SolidColorBrush(color);

        throw new ArgumentException("Parameter value must be a Windows.UI.Color");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is SolidColorBrush brush)
            return brush.Color;

        throw new ArgumentException("Parameter value must be a Microsoft.UI.Xaml.Media.SolidColorBrush");
    }
}
