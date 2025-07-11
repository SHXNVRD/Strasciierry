﻿using Microsoft.UI.Xaml;
using Strasciierry.UI.Services.Navigation;
using ImageConverterViewModel = Strasciierry.UI.Pages.ImageConverter.ImageConverterViewModel;

namespace Strasciierry.UI.Services.Activation.Handlers;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(ImageConverterViewModel).FullName!, args.Arguments);
        await Task.CompletedTask;
    }
}
