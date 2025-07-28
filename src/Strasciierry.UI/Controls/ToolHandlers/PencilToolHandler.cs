using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.ToolHandlers;

public class PencilToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!pointProps.IsLeftButtonPressed)
            return;

        UpdateCell(cell);
    }

    public override void HandlePointerEntered(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!e.Pointer.IsInContact || !pointProps.IsLeftButtonPressed)
            return;

        UpdateCell(cell);
    }

    private void UpdateCell(AsciiCanvasCell cell)
    {
        cell.Character = canvas.DrawingChar;
        cell.Foreground = canvas.DrawingForeground;
        cell.Background = canvas.DrawingBackground;
        cell.FontFamily = canvas.DrawingFontFamily;
        cell.FontStyle = canvas.DrawingFontStyle;
    }
}