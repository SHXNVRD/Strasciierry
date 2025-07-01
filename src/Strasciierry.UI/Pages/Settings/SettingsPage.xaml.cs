using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Pages.Settings;

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

    private async void TextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.SetUsersSymbolsAsync();
    }
}
