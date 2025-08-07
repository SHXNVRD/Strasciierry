using Microsoft.UI.Xaml;
using WinRT.Strasciierry_UIGenericHelpers;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class EraserToolHandler : ToolHandler
{
    private readonly AsciiCanvasCell _clearCell;

    public EraserToolHandler(IAsciiCanvas canvas)
        : base(canvas)
    {
        _clearCell = canvas.GetDefaultCell();
    }

    public override void Handle(ToolHandlerContext context)
    {
        var eventArgs = context.PointerEventArgs;
        var pointerProps = eventArgs.GetCurrentPoint((UIElement)canvas).Properties;

        switch (context.PointerEvent)
        {
            case PointerEvent.Pressed when pointerProps.IsLeftButtonPressed:
                ClearCell(context.CellColumn, context.CellRow);
                break;
            case PointerEvent.Entered when eventArgs.Pointer.IsInContact && pointerProps.IsLeftButtonPressed:
                ClearCell(context.CellColumn, context.CellRow);
                break;
            default:
                break;
        }
    }

    public void ClearCell(int column, int row)
    {
        var cell = canvas.GetCell(column, row);
        cell.Update(_clearCell);
    }
}