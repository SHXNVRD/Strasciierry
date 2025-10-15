using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.Core;
using Strasciierry.UI.Services.Activation.Handlers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Localization;
using Strasciierry.UI.Views;

namespace Strasciierry.UI.Services.Activation;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private UIElement? _shell = null;

    public ActivationService(
        ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        await InitializeAsync();

        if (App.MainWindow.Content == null)
        {
            _shell = KeyedIoc.Instance.GetRequiredService<ShellPage>();
            App.MainWindow.Content = _shell;
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
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await Task.CompletedTask;
    }
}
