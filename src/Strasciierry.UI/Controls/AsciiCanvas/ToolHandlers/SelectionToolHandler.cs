using Microsoft.UI.Xaml;
using Windows.System;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class SelectionToolHandler : ToolHandler
{
    public SelectionToolHandler(AsciiCanvas canvas)
        :base(canvas)
    { }

    public override void Handle(ToolHandlerContext context)
    {
        var eventArgs = context.PointerEventArgs;
        var pointerProps = eventArgs.GetCurrentPoint((UIElement)canvas).Properties;

        switch (context.PointerEvent)
        {
            case PointerEvent.Pressed when (eventArgs.KeyModifiers & VirtualKeyModifiers.Shift) != 0 && pointerProps.IsLeftButtonPressed:
                UpdateSelection(context.CellColumn, context.CellRow);
                break;
            case PointerEvent.Pressed when pointerProps.IsLeftButtonPressed:
                SetNewSelection(context.CellColumn, context.CellRow);
                break;
            case PointerEvent.Entered when eventArgs.Pointer.IsInContact && pointerProps.IsLeftButtonPressed:
                UpdateSelection(context.CellColumn, context.CellRow);
                break;
            default:
                break;
        }
    }

    private void UpdateSelection(int column, int row)
    {
        var currentSelection = canvas.Selection;
        canvas.SetSelection(new Selection
        {
            InitialColumn = currentSelection.InitialColumn,
            InitialRow = currentSelection.InitialRow,
            StartColumn = Math.Min(column, currentSelection.InitialColumn),
            StartRow = Math.Min(row, currentSelection.InitialRow),
            Columns = Math.Abs(currentSelection.InitialColumn - column) + 1,
            Rows = Math.Abs(currentSelection.InitialRow - row) + 1
        });
    }

    private void SetNewSelection(int column, int row)
    {
        canvas.SetSelection(new Selection
        {
            InitialColumn = column,
            InitialRow = row,
            StartColumn = column,
            StartRow = row,
            Columns = 1,
            Rows = 1
        });
    }
}
