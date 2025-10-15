using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Strasciierry.UI.Helpers;

namespace Strasciierry.UI.Services.Settings;

public partial class AppSettings : ObservableObject
{
    public string Version { get; } = RuntimeHelper.GetVersionDescription();

    [ObservableProperty] public partial GeneralSettings GeneralSettings { get; set; } = new();
    [ObservableProperty] public partial ImageToSymbolsSettings ImageToSymbolsSettings { get; set; } = new();
}
