using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.ToolHandlers.Base;

public abstract class ToolHandler : 
    IPointerPressedHandler,
    IPointerEnteredHandler
{
    public virtual void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e) { }
    public virtual void HandlePointerEntered(AsciiCanvasCell cell, PointerRoutedEventArgs e) { }
}
