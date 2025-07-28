using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Controls;

public class UserControlBase : UserControl
{
    public void SetCursor(InputCursor cursor)
    {
        ProtectedCursor = cursor;
    }
}
