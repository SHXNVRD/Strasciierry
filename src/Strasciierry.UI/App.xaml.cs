using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Serilog;
using Strasciierry.Core.Services.Files;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Services.Activation;
using Strasciierry.UI.Services.Activation.Handlers;
using Strasciierry.UI.Services.FilePicker;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.ImageToChars;
using Strasciierry.UI.Services.Navigation;
using Strasciierry.UI.Services.Pages;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.Services.Theme;
using Strasciierry.UI.Services.UsersSymbols;
using ImageConverterPage = Strasciierry.UI.Pages.ImageConverter.ImageConverterPage;
using ImageConverterViewModel = Strasciierry.UI.Pages.ImageConverter.ImageConverterViewModel;
using SettingsPage = Strasciierry.UI.Pages.Settings.SettingsPage;
using SettingsViewModel = Strasciierry.UI.Pages.Settings.SettingsViewModel;
using ShellPage = Strasciierry.UI.Pages.Shell.ShellPage;
using ShellViewModel = Strasciierry.UI.Pages.Shell.ShellViewModel;

namespace Strasciierry.UI;

public partial class App : Application
{
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

        var config = Host.Services.GetRequiredService<IConfiguration>();
        var appDataFolder = config["ApplicationLogsFolder"] ?? "Strasciierry/Logs";
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(localAppDataFolder, appDataFolder, "log-.txt"), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    private async void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Fatal("[EXCEPTION] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);

        await DialogHelper.ShowErrorAsync(App.Root.XamlRoot, $"{ex.Message}\n{ex}");
    }

    private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        var ex = e.Exception;
        Log.Fatal("[EXCEPTION] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
                ex.GetType().Name, ex.Message, ex, ex.InnerException);

        await DialogHelper.ShowErrorAsync(App.Root.XamlRoot, $"{e.Message}\n{e.Exception}");
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
        Root = (FrameworkElement)MainWindow.Content;
    }
}
