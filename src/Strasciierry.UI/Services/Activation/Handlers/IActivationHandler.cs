namespace Strasciierry.UI.Services.Activation.Handlers;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}
