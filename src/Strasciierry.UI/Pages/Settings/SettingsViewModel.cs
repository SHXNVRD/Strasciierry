using System.Reflection;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.Services.Theme;
using Strasciierry.UI.Services.UsersSymbols;

namespace Strasciierry.UI.Pages.Settings;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IUsersSymbolsService _userSymbolsService;
    private readonly IFontsService _fontsService;
    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    public partial ElementTheme ElementTheme { get; set; }

    [ObservableProperty]
    public partial string VersionDescription { get; set; }

    [ObservableProperty]
    public partial bool UsersSymbolsOn { get; set; }

    [ObservableProperty]
    public partial string UsersSymbols { get; set; }

    [ObservableProperty]
    public partial bool ShowMonospacedFontsOnly { get; set; }

    public SettingsViewModel(
        IThemeSelectorService themeSelectorService, 
        IUsersSymbolsService userSymbolsService,
        IFontsService fontsService,
        ILocalSettingsService localSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _userSymbolsService = userSymbolsService;
        _fontsService = fontsService;
        _localSettingsService = localSettingsService;

        ElementTheme = _themeSelectorService.Theme;
        UsersSymbols = new string(_userSymbolsService.UsersSymbols);
        UsersSymbolsOn = _userSymbolsService.UsersSymbolsOn;
        ShowMonospacedFontsOnly = _fontsService.ShowMonospacedFonstOnly;
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
    public async Task SetUsersSymbolsAsync()
    {
        if (string.IsNullOrWhiteSpace(UsersSymbols))
            return;

        var cleanSymbols = UsersSymbols.Where(c => !char.IsWhiteSpace(c)).ToArray();

        if (_userSymbolsService.UsersSymbols != cleanSymbols)
        {
            await _userSymbolsService.SetUsersSymbolsAsync(cleanSymbols);
            UsersSymbols = new string(_userSymbolsService.UsersSymbols);
        }
    }

    [RelayCommand]
    public async Task SetUsersSymbolsOnAsync()
        // Между заданием значения свойства ToggleSwitch.IsOn и передачи его IsUserSymbolsOn
        // есть задержка, во время которой значение IsUserSymbolsOn все ещё равно старому значению.
        // Это баг элемента управления.
        => await _userSymbolsService.SetUsersSymbolsOnAsync(!UsersSymbolsOn);

    [RelayCommand]
    public async Task SetShowMonospacedFontsOnly()
        => await _fontsService.SetShowMonospacedFontsOnly(!ShowMonospacedFontsOnly);

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
