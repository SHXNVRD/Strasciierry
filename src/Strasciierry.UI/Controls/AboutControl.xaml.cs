using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.Core;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Controls;

public sealed partial class AboutControl : UserControl
{
    public AboutControlViewModel ViewModel { get; } = KeyedIoc.Instance.GetRequiredService<AboutControlViewModel>();

    public AboutControl()
    {
        InitializeComponent();
    }
}
