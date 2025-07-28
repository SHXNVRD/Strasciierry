using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.ToolHandlers.Base;

public interface IPointerPressedHandler
{
    void HandlePointerPressed(AsciiCanvasCell cell, PointerRoutedEventArgs e);
}
