using Strasciierry.UI.Services.Settings;
using Windows.Storage;
using WinUI3Localizer;

namespace Strasciierry.UI.Services.Localization;

public class LocalizationService : ILocalizationService
{
    private const string SettingsKey = "AppLocalizationLanguage";

    private ILocalizer _localizer = Localizer.Get();

    private readonly ILocalSettingsService _localSettingsService;

    public LocalizationService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        await InitializeLocalizer();

        if (await LoadLanguageFromSettingsAsync() is string language)
        {
            await _localizer.SetLanguage(language);
        }
    }

    public async Task SetLanguageAsync(string language)
    {
        await _localizer.SetLanguage(language);
        await SaveLanguageInSettingsAsync(language);
    }

    public string GetCurrentLanguage() => _localizer.GetCurrentLanguage();

    public IEnumerable<string> GetAvailableLanguages() => _localizer.GetAvailableLanguages();

    public string GetLocalizedString(string uid) => _localizer.GetLocalizedString(uid);

    public IEnumerable<string> GetLocalizedStrings(string uid) => _localizer.GetLocalizedStrings(uid);

    private async Task InitializeLocalizer()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        var stringsFolder = await localFolder.CreateFolderAsync("Strings", CreationCollisionOption.OpenIfExists);

        await MakeSureStringResourceFileExists(stringsFolder, "en-us", "Resources.resw");
        await MakeSureStringResourceFileExists(stringsFolder, "ru-RU", "Resources.resw");

        _localizer = await new LocalizerBuilder()
            .AddStringResourcesFolderForLanguageDictionaries(stringsFolder.Path)
            .SetOptions(options =>
            {
                options.DefaultLanguage = "ru-RU";
            })
            .Build();
    }

    private static async Task MakeSureStringResourceFileExists(StorageFolder stringsFolder, string language, string resourceFileName)
    {
        var languageFolder = await stringsFolder.CreateFolderAsync(
            desiredName: language,
            CreationCollisionOption.OpenIfExists);

        var appResourceFilePath = Path.Combine(stringsFolder.Name, language, resourceFileName);
        var appResourceFile = await LoadStringResourcesFileFromAppResource(appResourceFilePath);

        var localResourceFile = await languageFolder.TryGetItemAsync(resourceFileName);

        if (localResourceFile is null ||
            (await GetModifiedDate(appResourceFile)) > (await GetModifiedDate(localResourceFile)))
        {
            _ = await appResourceFile.CopyAsync(
                destinationFolder: languageFolder,
                desiredNewName: appResourceFile.Name,
                option: NameCollisionOption.ReplaceExisting);
        }
    }

    private static async Task<StorageFile> LoadStringResourcesFileFromAppResource(string filePath)
    {
        Uri resourcesFileUri = new($"ms-appx:///{filePath}");
        return await StorageFile.GetFileFromApplicationUriAsync(resourcesFileUri);
    }

    private static async Task<DateTimeOffset> GetModifiedDate(IStorageItem file)
    {
        return (await file.GetBasicPropertiesAsync()).DateModified;
    }

    private async Task<string?> LoadLanguageFromSettingsAsync()
    {
        return await _localSettingsService.ReadSettingAsync<string>(SettingsKey);
    }

    private async Task SaveLanguageInSettingsAsync(string language)
    {
        await _localSettingsService.SaveSettingAsync(SettingsKey, language);
    }
}