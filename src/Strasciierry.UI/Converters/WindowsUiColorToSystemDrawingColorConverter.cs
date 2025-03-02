using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace Strasciierry.UI.Converters;

public class WindowsUiColorToSystemDrawingColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is System.Drawing.Color color)
        {
            return Windows.UI.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        throw new ArgumentException("Parameter value must be a System.Drawing.Color");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Windows.UI.Color color)
        {
            return System.Drawing.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        throw new ArgumentException("Parameter value must be a Windows.UI.Color");
    }
}
