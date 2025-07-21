using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace Strasciierry.UI.Converters;
internal class DoubleToThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not double doubleValue)
            throw new ArgumentException($"{nameof(value)} must be {typeof(double)}");

        return new Thickness(doubleValue);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) 
        => throw new NotImplementedException();
}
