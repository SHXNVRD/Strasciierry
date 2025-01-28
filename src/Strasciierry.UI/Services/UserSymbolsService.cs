using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strasciierry.UI.Contracts.Services;

namespace Strasciierry.UI.Services;
internal class UserSymbolsService : IUserSymbolsService
{
    private readonly ILocalSettingsService _settingsService;
    private bool _isInitialized;

    const string UserSymbolsSettingsKey = "UserSymbols";
    const string UseUserSymbolsSettingsKey = "IsUserSymbolsOn";

    public char[]? UserSymbols
    {
        get; private set;
    }
    public bool IsUserSymbolsOn
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
            IsUserSymbolsOn = await _settingsService.ReadSettingAsync<bool>(UseUserSymbolsSettingsKey);
            _isInitialized = true;
        }
    }

    public async Task SetUserSymbolsAsync(char[] symbols)
    {
        UserSymbols = symbols;

        await SaveUserSymbolsAsync(UserSymbols);
    }

    public async Task SetIsUserSymbolsOnAsync(bool useUserSymbols)
    {
        IsUserSymbolsOn = useUserSymbols;

        await SaveUseUserSymbolsAsync(IsUserSymbolsOn);
    }

    private async Task SaveUserSymbolsAsync(char[] symbols)
    {
        await _settingsService.SaveSettingAsync<char[]>(UserSymbolsSettingsKey, symbols);
    }

    private async Task SaveUseUserSymbolsAsync(bool useUserSymbols)
    {
        await _settingsService.SaveSettingAsync<bool>(UseUserSymbolsSettingsKey, useUserSymbols);
    }
}