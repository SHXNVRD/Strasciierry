using CommunityToolkit.Mvvm.ComponentModel;
using Strasciierry.UI.Services.Navigation;

namespace Strasciierry.UI.ViewModels;

public partial class ShellViewModel : ViewModelBase
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
