using Microsoft.UI.Xaml;

namespace Strasciierry.UI.Helpers;

public static class ThemeHelper
{
    public static void ChangeTheme(ElementTheme theme)
    {
        if (App.MainWindow.Content is FrameworkElement frame)
        {
            frame.RequestedTheme = theme;
            TitleBarHelper.UpdateTitleBar(theme);
        }
    }
}
