using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Controls;

public sealed partial class AboutControl : UserControl
{
    public AboutControlViewModel ViewModel { get; } = Ioc.Default.GetRequiredService<AboutControlViewModel>();

    public AboutControl()
    {
        InitializeComponent();
    }
}
