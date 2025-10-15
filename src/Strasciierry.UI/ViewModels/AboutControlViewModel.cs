using Strasciierry.UI.Services.Settings;

namespace Strasciierry.UI.ViewModels;

public class AboutControlViewModel : ViewModelBase
{
    private readonly ILocalSettingsService _settingsService;

    public AppSettings AppSettings { get; }

    public AboutControlViewModel(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
        AppSettings = _settingsService.AppSettings;
    }
}
