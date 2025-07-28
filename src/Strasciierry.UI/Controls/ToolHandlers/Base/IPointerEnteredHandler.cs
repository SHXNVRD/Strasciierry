using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.ToolHandlers.Base;

public interface IPointerEnteredHandler
{
    void HandlePointerEntered(AsciiCanvasCell cell, PointerRoutedEventArgs e);
    void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e);
}