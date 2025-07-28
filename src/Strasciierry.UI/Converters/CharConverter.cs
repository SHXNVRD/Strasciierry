using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Windows.UI;

namespace Strasciierry.UI.Converters;

public class CharConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var convertedValue = value is char c ? c : default(char);
        return convertedValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
