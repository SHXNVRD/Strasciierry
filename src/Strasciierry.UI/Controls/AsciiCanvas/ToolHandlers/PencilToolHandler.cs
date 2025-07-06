using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class PencilToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e)
        => cell.Character = canvas.DrawingChar;

    public override void HandlePointerEntered(CharCell cell, PointerRoutedEventArgs e)
    {
        if (e.Pointer.IsInContact)
            cell.Character = canvas.DrawingChar;
    }
}