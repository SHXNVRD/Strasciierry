using System.Reflection;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Settings;
using Strasciierry.Core.Services;
using Strasciierry.Core;

namespace Strasciierry.UI.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly ILocalSettingsService _settingsService;

    [ObservableProperty]
    public partial AppSettings AppSettings {get; set;}

    public SettingsPageViewModel(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
        AppSettings = _settingsService.AppSettings;
    }
}
