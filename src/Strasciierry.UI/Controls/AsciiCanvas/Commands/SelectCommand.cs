using Windows.System;
using Microsoft.UI.Xaml;
using Strasciierry.Core.Commands;

namespace Strasciierry.UI.Controls.AsciiCanvas.Commands;

public class SelectCommand(IAsciiCanvas canvas, GraphicToolContext context) : ICommand
{
    private Selection? _oldSelection;
    
    public Task Do()
    {
        _oldSelection = canvas.Selection;
        
        var eventArgs = context.PointerEventArgs;
        var pointerProps = eventArgs.GetCurrentPoint((UIElement)canvas).Properties;

        switch (context.PointerEvent)
        {
            case PointerEvent.Pressed when (eventArgs.KeyModifiers & VirtualKeyModifiers.Shift) != 0 && pointerProps.IsLeftButtonPressed:
                UpdateSelection(context.Column, context.Row);
                break;
            case PointerEvent.Pressed when pointerProps.IsLeftButtonPressed:
                SetNewSelection(context.Column, context.Row);
                break;
            case PointerEvent.Entered when eventArgs.Pointer.IsInContact && pointerProps.IsLeftButtonPressed:
                UpdateSelection(context.Column, context.Row);
                break;
        }

        return Task.CompletedTask;
    }

    public Task Undo()
    {
        if (_oldSelection.HasValue)
        {
            canvas.SetSelection(_oldSelection.Value);
        }

        return Task.CompletedTask;
    }
    
    private void UpdateSelection(int column, int row)
    {
        if (_oldSelection.HasValue)
        {
            canvas.SetSelection(new Selection
            {
                InitialColumn = _oldSelection.Value.InitialColumn,
                InitialRow = _oldSelection.Value.InitialRow,
                StartColumn = Math.Min(column, _oldSelection.Value.InitialColumn),
                StartRow = Math.Min(row, _oldSelection.Value.InitialRow),
                Columns = Math.Abs(_oldSelection.Value.InitialColumn - column) + 1,
                Rows = Math.Abs(_oldSelection.Value.InitialRow - row) + 1
            });
        }
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
