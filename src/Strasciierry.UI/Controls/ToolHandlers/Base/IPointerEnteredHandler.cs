using Microsoft.UI.Xaml.Input;

namespace Strasciierry.UI.Controls.ToolHandlers.Base;

public interface IPointerEnteredHandler
{
    void HandlePointerEntered(CharCell cell, PointerRoutedEventArgs e);
    void HandlePointerPressed(CharCell cell, PointerRoutedEventArgs e);
}