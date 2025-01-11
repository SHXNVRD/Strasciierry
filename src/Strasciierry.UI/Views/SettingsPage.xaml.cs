using Microsoft.UI.Xaml.Controls;

using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }
}
