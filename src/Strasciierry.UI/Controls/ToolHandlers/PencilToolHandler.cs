using Microsoft.UI.Xaml.Input;
using Strasciierry.UI.Controls.ToolHandlers.Base;

namespace Strasciierry.UI.Controls.ToolHandlers;

public class PencilToolHandler(AsciiCanvas canvas) : ToolHandler
{
    public override void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!pointProps.IsLeftButtonPressed)
            return;
        
        cell.Character = canvas.DrawingChar;
    }

    public override void HandlePointerEntered(CharCell cell, PointerRoutedEventArgs e)
    {
        var pointProps = e.GetCurrentPoint(canvas).Properties;

        if (!e.Pointer.IsInContact || !pointProps.IsLeftButtonPressed)
            return;

        cell.Character = canvas.DrawingChar;
    }
}