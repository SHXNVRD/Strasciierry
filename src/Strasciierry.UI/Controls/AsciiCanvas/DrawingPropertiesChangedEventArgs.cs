using System.Drawing;

namespace Strasciierry.UI.Controls.AsciiCanvas;

public class DrawingPropertiesChangedEventArgs : EventArgs
{
    public char Character { get; }
    public Color Foreground { get; }
    public Color Background { get; }
    public FontFamily FontFamily { get; }
    public FontStyle FontStyle { get; }

    public DrawingPropertiesChangedEventArgs(
        char character, 
        Color foreground, 
        Color background, 
        FontFamily fontFamily, 
        FontStyle fontStyle)
    {
        Character = character;
        Foreground = foreground;
        Background = background;
        FontFamily = fontFamily;
        FontStyle = fontStyle;
    }
}
