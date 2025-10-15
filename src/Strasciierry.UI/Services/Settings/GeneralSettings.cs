using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Strasciierry.UI.Services.Settings;

public partial class GeneralSettings : ObservableObject
{
    [ObservableProperty] public partial ElementTheme AppTheme { get; set; } = ElementTheme.Default;
}
