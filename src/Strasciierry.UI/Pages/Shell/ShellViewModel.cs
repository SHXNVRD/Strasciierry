using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Navigation;
using Strasciierry.UI.Pages.Settings;
using Strasciierry.UI.Services.Navigation;

namespace Strasciierry.UI.Pages.Shell;

public partial class ShellViewModel : ObservableRecipient
{
    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}
