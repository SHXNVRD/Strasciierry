using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Controls;

public abstract class UserControlBase : UserControl
{
    public virtual void SetCursor(InputCursor cursor)
    {
        ProtectedCursor = cursor;
    }
}
