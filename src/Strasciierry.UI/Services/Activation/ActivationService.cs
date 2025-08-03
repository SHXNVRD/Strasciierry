using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.Services.Activation.Handlers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Localization;
using Strasciierry.UI.Services.Theme;
using Strasciierry.UI.Services.UsersSymbols;
using Strasciierry.UI.Views;

namespace Strasciierry.UI.Services.Activation;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IUsersSymbolsService _userSymbolsService;
    private readonly IFontsService _fontsService;
    private readonly ILocalizationService _localizationService;
    private UIElement? _shell = null;

    public ActivationService(
        ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IThemeSelectorService themeSelectorService,
        IUsersSymbolsService userSymbolsService,
        IFontsService fontsService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _userSymbolsService = userSymbolsService;
        _fontsService = fontsService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        await InitializeAsync();

        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? throw new NullReferenceException($"{typeof(ShellPage)} is not registered");
        }

        await HandleActivationAsync(activationArgs);
        App.MainWindow.Activate();
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await _userSymbolsService.InitializeAsync().ConfigureAwait(false);
        _fontsService.Initialize();
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }
}
