using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Helpers;
internal class PercentageDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => $"{value} %";
    public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
}
