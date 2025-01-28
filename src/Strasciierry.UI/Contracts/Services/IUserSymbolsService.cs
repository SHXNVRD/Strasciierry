namespace Strasciierry.UI.Contracts.Services;

public interface IUserSymbolsService
{
    char[]? UserSymbols { get; } 
    bool IsUserSymbolsOn { get; }

    Task InitializeAsync();
    Task SetUserSymbolsAsync(char[] symbols);
    Task SetIsUserSymbolsOnAsync(bool useUserSymbols);
}