using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.AsciiCanvas.ToolHandlers.Base;

public abstract class ToolHandler : 
    IPointerPressedHandler,
    IPointerEnteredHandler
{
    public virtual void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e) { }
    public virtual void HandlePointerEntered(CharCell cell, PointerRoutedEventArgs e) { }
}
