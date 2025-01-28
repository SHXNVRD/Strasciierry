using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Helpers;

using Windows.ApplicationModel;

namespace Strasciierry.UI.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IUserSymbolsService _userSymbolsService;

    [ObservableProperty]
    public partial ElementTheme ElementTheme { get; set; }

    [ObservableProperty]
    public partial string VersionDescription { get; set; }

    [ObservableProperty]
    public partial bool IsUserSymbolsOn { get; set; }

    [ObservableProperty]
    public partial string UserSymbols { get; set; }


    public SettingsViewModel(IThemeSelectorService themeSelectorService, IUserSymbolsService userSymbolsService)
    {
        _themeSelectorService = themeSelectorService;
        _userSymbolsService = userSymbolsService;
        ElementTheme = _themeSelectorService.Theme;
        UserSymbols = new string(_userSymbolsService.UserSymbols);
        IsUserSymbolsOn = _userSymbolsService.IsUserSymbolsOn;
        VersionDescription = GetVersionDescription();
    }

    [RelayCommand]
    public async Task SwitchThemeAsync(ElementTheme theme)
    {
        if (ElementTheme != theme)
        {
            ElementTheme = theme;
            await _themeSelectorService.SetThemeAsync(theme);
        }
    }

    [RelayCommand]
    public async Task SetUserSymbolsAsync()
    {
        if (string.IsNullOrWhiteSpace(UserSymbols))
            return;

        var cleanSymbols = UserSymbols.Where(c => !char.IsWhiteSpace(c)).ToArray();

        if (_userSymbolsService.UserSymbols != cleanSymbols)
        {
            await _userSymbolsService.SetUserSymbolsAsync(cleanSymbols);
            UserSymbols = new string(_userSymbolsService.UserSymbols);
        }
    }

    [RelayCommand]
    public async Task SetIsUserSymbolsOnAsync() =>
        await _userSymbolsService.SetIsUserSymbolsOnAsync(!IsUserSymbolsOn);

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
