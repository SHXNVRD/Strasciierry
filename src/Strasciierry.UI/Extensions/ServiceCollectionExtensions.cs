using System.Drawing;
using System.Drawing.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Serilog;
using Strasciierry.Core.Helpers;
using Strasciierry.Core.Services;
using Strasciierry.UI.Controls.AsciiCanvas.Commands;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.Factories;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Helpers.SaveArtStrategies;
using Strasciierry.UI.Services.Activation;
using Strasciierry.UI.Services.Activation.Handlers;
using Strasciierry.UI.Services.Fonts;
using Strasciierry.UI.Services.ImageToSymbols;
using Strasciierry.UI.Services.Navigation;
using Strasciierry.UI.Services.Pages;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.Services.Theme;
using Strasciierry.UI.Services.UsersSymbols;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Views;

namespace Strasciierry.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {

        services
            .ConfigureJson()
            .AddLogging()
            .AddSaveArtStrategies()
            .AddServices()
            .AddViews();

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var config = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

        var appDataFolder = config["ApplicationLogsFolder"] ?? "Strasciierry/Logs";
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var logsFolder = Path.Combine(localAppDataFolder, appDataFolder, "log-.txt");

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.Console()
            .WriteTo.File(logsFolder, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        return services;
    }

    private static IServiceCollection AddSaveArtStrategies(this IServiceCollection services)
    {
        services
            .AddKeyedTransient<SaveArtStrategy, BmpSaveArtStrategy>(FilePickerHelper.Bmp.Key)
            .AddKeyedTransient<SaveArtStrategy, EmfSaveArtStrategy>(FilePickerHelper.Emf.Key)
            .AddKeyedTransient<SaveArtStrategy, GifSaveArtStrategy>(FilePickerHelper.Gif.Key)
            .AddKeyedTransient<SaveArtStrategy, HeifSaveArtStrategy>(FilePickerHelper.Heif.Key)
            .AddKeyedTransient<SaveArtStrategy, IcoSaveArtStrategy>(FilePickerHelper.Ico.Key)
            .AddKeyedTransient<SaveArtStrategy, JpgSaveArtStrategy>(FilePickerHelper.Jpg.Key)
            .AddKeyedTransient<SaveArtStrategy, PngSaveArtStrategy>(FilePickerHelper.Png.Key)
            .AddKeyedTransient<SaveArtStrategy, TiffSaveArtStrategy>(FilePickerHelper.Tiff.Key)
            .AddKeyedTransient<SaveArtStrategy, TxtSaveArtStrategy>(FilePickerHelper.Txt.Key)
            .AddKeyedTransient<SaveArtStrategy, WebpSaveArtStrategy>(FilePickerHelper.Webp.Key)
            .AddKeyedSingleton<SaveArtStrategy, WmfSaveArtStrategy>(FilePickerHelper.Wmf.Key);

        services.AddTransient<ISaveArtStrategyFactory, SaveArtStrategyFactory>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        var config = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

        services.Configure<LocalSettingsOptions>(config.GetSection(nameof(LocalSettingsOptions)));
        services.AddSingleton<ILocalSettingsService, LocalSettingsService>();

        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<IUserSymbolsService, UserSymbolsService>();
        services.AddSingleton<IFontsService, FontsService>();
        services.AddTransient<IImageToSymbolsService, ImageToSymbolsService>();

        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IGraphicToolCommandFactory, GraphicToolCommandFactory>();

        services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();
        services.AddSingleton<IActivationService, ActivationService>();

        return services;
    }

    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<ShellPage>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<AsciiArtPageViewModel>();
        services.AddTransient<AsciiArtPage>();
        services.AddTransient<CharacterPaletteItemEditDialog>();

        return services;
    }

    private static IServiceCollection ConfigureJson(this IServiceCollection services)
    {
        services.AddKeyedTransient<JsonConverter, FontFamilyJsonConverter>(typeof(FontFamily));

        JsonConvert.DefaultSettings = CreateSettings;

        static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new ConverterContractResolver()
            };
        }

        return services;
    }
}