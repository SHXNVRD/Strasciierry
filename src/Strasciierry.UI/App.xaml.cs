using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Strasciierry.Core.Contracts.Services;
using Strasciierry.UI.Activation;
using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Core.Services;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Options;
using Strasciierry.UI.Services;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Views;

namespace Strasciierry.UI;

public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host { get; }

    public static WindowEx MainWindow { get; } = new MainWindow();
    public static FrameworkElement Root { get; private set; }

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");

        return service;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
                services.Configure<ImageToCharOptions>(context.Configuration.GetSection(nameof(ImageToCharOptions)));
                services.Configure<FilePickerOptions>(context.Configuration.GetSection(nameof(FilePickerOptions)));

                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddSingleton<IFilePickerService, FilePickerService>();
                services.AddSingleton<IUsersSymbolsService, UsersSymbolsService>();
                services.AddSingleton<IFontsService, FontsService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddTransient<IImageToCharsService, ImageToCharsService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IFileService, FileService>();

                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ImageConverterViewModel>();
                services.AddTransient<ImageConverterPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<ImageConverterViewModel>();
                services.AddTransient<ImageConverterPage>();
            }).
            Build();

        UnhandledException += App_UnhandledException;
    }
        
    private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        await DialogHelper.ShowErrorAsync(App.Root.XamlRoot, $"{e.Message}\n{e.Exception}");
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
        Root = (FrameworkElement)MainWindow.Content;
    }
}
