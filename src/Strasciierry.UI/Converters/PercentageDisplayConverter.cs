using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Converters;
internal class PercentageDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => $"{value} %";
    public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
}
