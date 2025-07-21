using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Text;

namespace Strasciierry.UI.Controls;

public class Character
{
    public char Symbol { get; set; }
    public FontFamily FontFamily { get; set; } = FontFamily.XamlAutoFontFamily;
    public Color Foreground { get; set; } = Colors.White;
    public Color Background { get; set; } = Colors.Black;
    public TextDecorations TextDecorations = TextDecorations.None;

    public Character(char symbol = '*')
    {
        Symbol = symbol;
    }
}
