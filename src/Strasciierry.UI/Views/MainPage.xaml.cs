using Microsoft.UI.Xaml.Controls;

using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
