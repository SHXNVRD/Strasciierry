using Microsoft.UI.Xaml;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.Settings;

namespace Strasciierry.UI.Services.Activation.Handlers;

public class ThemeActivationHandler(ILocalSettingsService settingsService) : ActivationHandler<ElementTheme>
{
    protected override bool CanHandleInternal(ElementTheme args) 
        => args != settingsService.AppSettings.GeneralSettings.AppTheme;

    protected override Task HandleInternalAsync(ElementTheme args)
    {
        ThemeHelper.ChangeTheme(settingsService.AppSettings.GeneralSettings.AppTheme);

        return Task.CompletedTask;
    }
}
