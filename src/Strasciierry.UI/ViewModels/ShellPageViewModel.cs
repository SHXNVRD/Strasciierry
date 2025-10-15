using CommunityToolkit.Mvvm.ComponentModel;
using Strasciierry.UI.Services.Navigation;

namespace Strasciierry.UI.ViewModels;

public partial class ShellPageViewModel : ViewModelBase
{
    public INavigationService NavigationService
    {
        get;
    }

    public ShellPageViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}
