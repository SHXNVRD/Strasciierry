using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers.Base;
using Windows.System;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class SelectionToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e)
    {
        if ((e.KeyModifiers & VirtualKeyModifiers.Shift) != 0)
            canvas.UpdateSelection(cell.Column, cell.Row);
        else
        {
            canvas.ClearSelection();
            canvas.UpdateSelection(cell.Column, cell.Row);
        }
    }

    public override void HandlePointerEntered(CharCell cell, PointerRoutedEventArgs e)
    {
        canvas.UpdateSelection(cell.Column, cell.Row);
    }
}
