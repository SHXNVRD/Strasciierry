using Microsoft.UI.Xaml;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class PencilToolHandler : ToolHandler
{
    public PencilToolHandler(AsciiCanvas canvas)
       : base(canvas)
    { }

    public override void Handle(ToolHandlerContext context)
    {
        var eventArgs = context.PointerEventArgs;
        var pointerProps = eventArgs.GetCurrentPoint((UIElement)canvas).Properties;

        switch (context.PointerEvent)
        {
            case PointerEvent.Pressed when pointerProps.IsLeftButtonPressed:
                StylizeCell(context.CellColumn, context.CellRow);
                break;
            case PointerEvent.Entered when eventArgs.Pointer.IsInContact && pointerProps.IsLeftButtonPressed:
                StylizeCell(context.CellColumn, context.CellRow);
                break;
            default:
                break;
        }
    }

    private void StylizeCell(int column, int row)
    {
        canvas
            .GetCell(column, row)
            .Update(canvas.GetStyledCell());
    }
}