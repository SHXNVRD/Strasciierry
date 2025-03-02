namespace Strasciierry.UI.Contracts.Services;

public interface ILocalSettingsService
{
    public const string UseMonospacedFontsOnlySettingsKey = "ShowMonospacedFontsOnly";

    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);
}
