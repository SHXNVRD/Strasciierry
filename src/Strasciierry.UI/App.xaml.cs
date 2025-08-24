using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Serilog;
using Strasciierry.Core.Services;
using Strasciierry.UI.Controls.AsciiCanvas.Commands;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.Activation;
using Strasciierry.UI.Services.Activation.Handlers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.Localization;
using Strasciierry.UI.Services.Navigation;
using Strasciierry.UI.Services.Pages;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.Services.Theme;
using Strasciierry.UI.Services.UsersSymbols;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Views;
using Strasciierry.UI.Extensions;
using Strasciierry.UI.Services.ImageToSymbols;
using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;

namespace Strasciierry.UI;

public partial class App : Application
{
    public IHost Host { get; }
    public static WindowEx MainWindow { get; } = new MainWindow();
    public  static new App Current => (App)Application.Current;
    public static XamlRoot XamlRoot => MainWindow.Content.XamlRoot;

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public static T GetService<T>()
        where T : class
    {
        if (Current.Host.Services.GetService(typeof(T)) is not T service)
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

                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                services.AddSaveArtStrategies();
                services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddSingleton<IUserSymbolsService, UserSymbolsService>();
                services.AddSingleton<IFontsService, FontsService>();
                services.AddTransient<IImageToSymbolsService, ImageToSymbolsService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<IGraphicToolCommandFactory, GraphicToolCommandFactory>();

                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<AsciiArtPageViewModel>();
                services.AddTransient<AsciiArtPage>();
                services.AddTransient<CharacterPaletteItemEditDialog>();
            }).
            Build();

        var config = Host.Services.GetRequiredService<IConfiguration>();
        var appDataFolder = config["ApplicationLogsFolder"] ?? "Strasciierry/Logs";
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(localAppDataFolder, appDataFolder, "log-.txt"), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        
    }

    private async void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Fatal("[EXCEPTION] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);

        await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{ex.Message}\n{ex}");
    }

    private async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Fatal("[EXCEPTION] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
                ex.GetType().Name, ex.Message, ex, ex.InnerException);

        await DialogHelper.ShowErrorAsync(App.XamlRoot, $"{e.Message}\n{e.Exception}");

        e.Handled = true;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await GetService<IActivationService>().ActivateAsync(args);
    }
}
