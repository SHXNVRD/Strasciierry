using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.ToolHandlers;

class PipetteToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        canvas.DrawingChar = cell.Character;
        canvas.DrawingForeground = cell.Foreground;
        canvas.DrawingBackground = cell.Background;
        canvas.DrawingFontFamily = cell.FontFamily;
        canvas.DrawingFontStyle = cell.FontStyle;
    }
}
