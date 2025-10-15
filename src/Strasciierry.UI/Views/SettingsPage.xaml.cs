using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.Core;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPageViewModel ViewModel { get; } = KeyedIoc.Instance.GetRequiredService<SettingsPageViewModel>();

    public SettingsPage()
    {
        InitializeComponent();
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        => ViewSwitchPresenter.Value = ((NavigationViewItem)args.SelectedItem).Tag;
}
