using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strasciierry.UI.Contracts.Services;

namespace Strasciierry.UI.Services;
internal class UsersSymbolsService : IUsersSymbolsService
{
    private readonly ILocalSettingsService _settingsService;
    private bool _isInitialized;

    const string UserSymbolsSettingsKey = "UsersSymbols";
    const string UseUserSymbolsSettingsKey = "UsersSymbolsOn";

    public char[]? UsersSymbols
    {
        get; private set;
    }
    public bool UsersSymbolsOn
    {
        get; private set;
    }

    public UsersSymbolsService(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            UsersSymbols = await _settingsService.ReadSettingAsync<char[]>(UserSymbolsSettingsKey);
            UsersSymbolsOn = await _settingsService.ReadSettingAsync<bool>(UseUserSymbolsSettingsKey);
            _isInitialized = true;
        }
    }

    public async Task SetUsersSymbolsAsync(char[] symbols)
    {
        UsersSymbols = symbols;

        await SaveUserSymbolsAsync(UsersSymbols);
    }

    public async Task SetUsersSymbolsOnAsync(bool useUserSymbols)
    {
        UsersSymbolsOn = useUserSymbols;

        await SaveUseUserSymbolsAsync(UsersSymbolsOn);
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