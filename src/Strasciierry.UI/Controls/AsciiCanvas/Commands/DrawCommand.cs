using Microsoft.UI.Xaml;
using Strasciierry.Core.Commands;

namespace Strasciierry.UI.Controls.AsciiCanvas.Commands;

public class DrawCommand(IAsciiCanvas canvas, GraphicToolContext context) : ICommand
{
    private AsciiCanvasCell? _oldCell;

    public Task Do()
    {
        var eventArgs = context.PointerEventArgs;
        var pointerProps = eventArgs.GetCurrentPoint((UIElement)canvas).Properties;

        switch (context.PointerEvent)
        {
            case PointerEvent.Pressed when pointerProps.IsLeftButtonPressed:
            case PointerEvent.Entered when eventArgs.Pointer.IsInContact && pointerProps.IsLeftButtonPressed:
                StylizeCell(context.Column, context.Row);
                break;
        }

        return Task.CompletedTask;
    }

    public Task Undo()
    {
        if (_oldCell is not null)
        {
            canvas
                .GetCell(context.Column, context.Row)
                .Update(_oldCell);
        }
        
        return Task.CompletedTask;
    }
    
    private void StylizeCell(int column, int row)
    {
        _oldCell = canvas
            .GetCell(column, row)
            .Clone();
        
        canvas
            .GetCell(column, row)
            .Update(canvas.GetStyledCell());
    }
}