using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers;

public class ToolHandlerContext
{
    public PointerEvent PointerEvent { get; init; }
    public PointerRoutedEventArgs PointerEventArgs  { get; init; }
    public int CellColumn { get; init; }
    public int CellRow { get; init; }
}
