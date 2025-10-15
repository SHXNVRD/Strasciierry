using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Strasciierry.UI.Views;
using Strasciierry.WinApi.User32;
using WinRT.Interop;

namespace Strasciierry.UI.Helpers;

public static class WindowHelper
{
    private static readonly List<Window> _activeWindows = [];

    public static void OpenWindow<T>() where T : Window
    {
        var window = _activeWindows.Find(w => w is T);
        if (window is null)
        {
            var windowType = typeof(T);
            if (windowType == typeof(SettingsWindow))
                window = new SettingsWindow();
            else
                throw new ArgumentException("Unsupported window type", nameof(T));
        }

        TrackWindow(window);
        ShowWindow(window);
    }

    public static void SetOwnership(AppWindow owned, Window owner)
    {
        var ownerHwnd = WindowNative.GetWindowHandle(owner);
        var ownedHwnd = Win32Interop.GetWindowFromWindowId(owned.Id);

        User32.SetWindowLong(ownedHwnd, -8, ownerHwnd);
    }

    private static void ShowWindow(Window window)
    {
        window.Restore();
        window.Activate();
    }

    private static void TrackWindow(Window window)
    {
        if (!_activeWindows.Contains(window))
        {
            _activeWindows.Add(window);
            window.Closed += WindowHelper_Closed;
        }
    }

    private static void WindowHelper_Closed(object sender, WindowEventArgs args)
    {
        var castedWindow = (Window)sender;

        if (_activeWindows.Contains(castedWindow))
            _activeWindows.Remove(castedWindow);
    }
}
