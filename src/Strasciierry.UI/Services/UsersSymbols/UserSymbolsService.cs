using System.Diagnostics.CodeAnalysis;
using Strasciierry.UI.Services.Settings;

namespace Strasciierry.UI.Services.UsersSymbols;
internal class UserSymbolsService : IUserSymbolsService
{
    private readonly ILocalSettingsService _settingsService;
    private bool _isInitialized;

    private const string UserSymbolsSettingsKey = "UserSymbols";
    private const string UserSymbolsNegativeSettingsKey = "UserSymbolsNegative";
    private const string UseUserSymbolsSettingsKey = "UsersSymbolsOn";

    public char[] DefaultSymbols { get; } = [.. ".,;+*?%S#@"];
    public char[] DefaultSymbolsNegative => DefaultSymbols.AsEnumerable().Reverse().ToArray();

    [field: AllowNull, MaybeNull]
    public char[] UserSymbols
    {
        get => field ?? DefaultSymbols;
        private set;
    }

    [field: AllowNull, MaybeNull]
    public char[] UserSymbolsNegative
    {
        get => field ?? UserSymbols.AsEnumerable().Reverse().ToArray();
        private set;
    }

    public bool UsersSymbolsOn
    {
        get; private set;
    }

    public UserSymbolsService(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            UserSymbols = await _settingsService.ReadSettingAsync<char[]>(UserSymbolsSettingsKey);
            UsersSymbolsOn = await _settingsService.ReadSettingAsync<bool>(UseUserSymbolsSettingsKey);
            _isInitialized = true;
        }
    }

    public async Task SetUserSymbolsAsync(char[] symbols)
    {
        UserSymbols = symbols;
        await _settingsService.SaveSettingAsync(UserSymbolsSettingsKey, symbols);
    }

    public async Task SetUserSymbolsNegativeAsync(char[] symbols)
    {
        UserSymbolsNegative = symbols;
        await _settingsService.SaveSettingAsync(UserSymbolsNegativeSettingsKey, symbols);
    }

    public async Task SetUserSymbolsOnAsync(bool useUserSymbols)
    {
        UsersSymbolsOn = useUserSymbols;
        await _settingsService.SaveSettingAsync(UseUserSymbolsSettingsKey, useUserSymbols);
    }
}