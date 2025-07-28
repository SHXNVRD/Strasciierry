using System.Drawing;
using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Converters;

internal class SystemDrawingColorToHexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not Color color)
            throw new ArgumentException($"Value must be of type {typeof(Color)}", nameof(value));

        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => throw new NotImplementedException();
}
