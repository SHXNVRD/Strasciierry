using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Converters;
internal class FactorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return $"{doubleValue.ToString("F1", CultureInfo.InvariantCulture)} x";
        }

        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
}
