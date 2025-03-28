namespace Strasciierry.UI.Services.UsersSymbols;

public interface IUsersSymbolsService
{
    char[]? UsersSymbols { get; }
    bool UsersSymbolsOn { get; }

    Task InitializeAsync();
    Task SetUsersSymbolsAsync(char[] symbols);
    Task SetUsersSymbolsOnAsync(bool useUserSymbols);
}