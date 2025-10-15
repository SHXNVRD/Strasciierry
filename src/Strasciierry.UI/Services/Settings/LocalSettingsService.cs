using Microsoft.Extensions.Options;
using Strasciierry.UI.Helpers;
using Windows.Storage;
using Strasciierry.Core.Services;
using Strasciierry.Core.Helpers;
using System.Xml;
using Strasciierry.UI.ViewModels;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace Strasciierry.UI.Services.Settings;

public class LocalSettingsService : ViewModelBase, ILocalSettingsService
{
    private const string DefaultApplicationDataFolder = "Strasciierry/ApplicationData";
    private const string DefaultLocalSettingsFile = "LocalSettings.json";

    private readonly string _applicationDataFolder;
    private readonly string _settingsFileName;
    private readonly LocalSettingsOptions _options;
    private readonly TimeSpan _debounceTimeout = TimeSpan.FromMilliseconds(300);

    public AppSettings AppSettings { get; }

    public LocalSettingsService(IOptions<LocalSettingsOptions> options)
    {
        _options = options.Value;

        var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _applicationDataFolder = Path.Combine(localApplicationData, _options.ApplicationDataFolder ?? DefaultApplicationDataFolder);
        _settingsFileName = _options.LocalSettingsFile ?? DefaultLocalSettingsFile;

        AppSettings = ReadAppSettings();

        AppSettings.GeneralSettings.PropertyChanged += GeneralSettings_PropertyChanged;
        AppSettings.ImageToSymbolsSettings.PropertyChanged += ImageToSymbolsSettings_PropertyChanged;
    }

    private void SaveAppSettingsDebounce()
    {
        _dispatcherQueueTimer.Debounce(() =>
        {
            _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, SaveAppSettings);
        }, _debounceTimeout);
    }

    private AppSettings ReadAppSettings()
        => FileHelper.Read<AppSettings>(_applicationDataFolder, _settingsFileName) ?? new AppSettings();

    // TODO: По возможности сохранять в ApplicationData.Current.LocalSettings.Values
    private void SaveAppSettings()
        => FileHelper.Save(_applicationDataFolder, _settingsFileName, AppSettings);

    private void GeneralSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GeneralSettings.AppTheme))
            ThemeHelper.ChangeTheme(AppSettings.GeneralSettings.AppTheme);

        SaveAppSettingsDebounce();
    }

    private void ImageToSymbolsSettings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => SaveAppSettingsDebounce();
}
