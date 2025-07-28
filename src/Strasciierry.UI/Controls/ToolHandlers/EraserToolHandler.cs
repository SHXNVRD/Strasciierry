using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.ToolHandlers;

public class EraserToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!pointProps.IsLeftButtonPressed)
            return;

        ClearCell(cell);
    }

    public override void HandlePointerEntered(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!e.Pointer.IsInContact || !pointProps.IsLeftButtonPressed)
            return;

        ClearCell(cell);
    }

    private void ClearCell(AsciiCanvasCell cell)
    {
        cell.Character = AsciiCanvas.DefaultCellCharacter;
        cell.Foreground = AsciiCanvas.DefaultDrawingForeground;
        cell.Background = AsciiCanvas.DefaultDrawingBackground;
        cell.FontFamily = AsciiCanvas.DefaultDrawingFontFamily;
        cell.FontStyle = AsciiCanvas.DefaultDrawingFontStyle;
    }
}