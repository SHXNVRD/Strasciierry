using CommunityToolkit.Mvvm.ComponentModel;
using Strasciierry.UI.Services.Settings;

namespace Strasciierry.UI.ViewModels;

public partial class GeneralSettingsControlViewModel : ViewModelBase
{
    private readonly ILocalSettingsService _settingsService;

    [ObservableProperty]
    public partial GeneralSettings GeneralSettings { get; set; }

    public GeneralSettingsControlViewModel(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
        GeneralSettings = _settingsService.AppSettings.GeneralSettings;
    }
}
