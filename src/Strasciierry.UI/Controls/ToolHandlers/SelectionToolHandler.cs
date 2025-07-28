using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;
using Windows.System;

namespace Strasciierry.UI.Controls.ToolHandlers;

public class SelectionToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(canvas).Properties;

        if ((e.KeyModifiers & VirtualKeyModifiers.Shift) != 0
            && properties.IsLeftButtonPressed)
        {
            canvas.UpdateSelection(cell.Column, cell.Row);
        }
        else if (properties.IsLeftButtonPressed)
        {
            canvas.ClearSelection();
            canvas.UpdateSelection(cell.Column, cell.Row);
        }
    }

    public override void HandlePointerEntered(AsciiCanvasCell cell, PointerRoutedEventArgs e)
    {
        canvas.UpdateSelection(cell.Column, cell.Row);
    }
}
