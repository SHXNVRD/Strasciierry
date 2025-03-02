using Microsoft.UI.Xaml.Data;
using Windows.UI.Text;

namespace Strasciierry.UI.Converters;

class WindowsStyleToDrawingFontStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => value;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (!Enum.IsDefined(typeof(FontStyle), value))
            throw new ArgumentException("Parameter value must be an enum");

        return (System.Drawing.FontStyle)value;
    }
}
