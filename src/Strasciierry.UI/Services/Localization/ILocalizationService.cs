namespace Strasciierry.UI.Services.Localization;

public interface ILocalizationService
{
    Task InitializeAsync();
    string GetCurrentLanguage();
    IEnumerable<string> GetAvailableLanguages();
    public string GetLocalizedString(string uid);
    public IEnumerable<string> GetLocalizedStrings(string uid);
    Task SetLanguageAsync(string language);
}
