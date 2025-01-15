namespace Strasciierry.UI.Contracts.Services;

public interface IUserSymbolsService
{
    char[]? UserSymbols { get; } 
    bool UseUserSymbols { get; }

    Task InitializeAsync();
    Task SetUserSymbolsAsync(string symbols);
    Task SetUseUserSymbolsAsync(bool useUserSymbols);
}