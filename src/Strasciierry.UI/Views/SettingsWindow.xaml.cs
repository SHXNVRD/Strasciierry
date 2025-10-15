using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Strasciierry.UI.Helpers;
using WinRT.Interop;

namespace Strasciierry.UI.Views;

public sealed partial class SettingsWindow : WindowEx
{
    public SettingsWindow()
    {
        InitializeComponent();

        RootFrame.Navigate(typeof(SettingsPage));

        var presenter = OverlappedPresenter.CreateForDialog();
        WindowHelper.SetOwnership(AppWindow, App.MainWindow);
        presenter.IsModal = true;
        AppWindow.SetPresenter(presenter);
    }
}