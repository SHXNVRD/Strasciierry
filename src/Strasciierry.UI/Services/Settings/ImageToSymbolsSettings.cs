using CommunityToolkit.Mvvm.ComponentModel;

namespace Strasciierry.UI.Services.Settings;

public partial class ImageToSymbolsSettings : ObservableObject
{
    [ObservableProperty] public partial char[] ImageToSymbolsCharacters { get; set; } = [.. ".,;+*?%S#@"];
    [ObservableProperty] public partial bool UseDefaultCharacters { get; set; } = true;
}
