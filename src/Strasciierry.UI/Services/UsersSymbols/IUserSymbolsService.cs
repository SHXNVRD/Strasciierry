namespace Strasciierry.UI.Services.UsersSymbols;

public interface IUserSymbolsService
{
    char[] DefaultSymbols { get; }
    char[] DefaultSymbolsNegative { get; }
    char[] UserSymbols { get; }
    char[] UserSymbolsNegative { get; }
    bool UsersSymbolsOn { get; }

    Task InitializeAsync();
    Task SetUserSymbolsAsync(char[] symbols);
    Task SetUserSymbolsNegativeAsync(char[] symbols);
    Task SetUserSymbolsOnAsync(bool useUserSymbols);
}