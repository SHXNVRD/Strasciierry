using System.Drawing;
using System.Drawing.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Serilog;
using Strasciierry.Core;
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
using Strasciierry.UI.Services.Navigation;
using Strasciierry.UI.Services.Pages;
using Strasciierry.UI.Services.Settings;
using Strasciierry.UI.ViewModels;
using Strasciierry.UI.Views;
using Microsoft.Extensions.Logging;

namespace Strasciierry.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services
            .AddConfiguration()
            .ConfigureJson()
            .AddLogging()
            .AddServices()
            .AddViewsAndViewModels();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false)
            .Build();

        services.AddSingleton<IConfiguration>(config);

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var config = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

        // TODO: По возможности писать в ApplicationData.Current.LocalFolder
        var appDataFolder = config["ApplicationLogsFolder"] ?? "Strasciierry/Logs";
        var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var logsFolder = Path.Combine(localAppDataFolder, appDataFolder, "log-.txt");

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.File(
            logsFolder, 
            rollingInterval: RollingInterval.Day, 
            rollOnFileSizeLimit: true, 
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder
            .ClearProviders()
            .AddSerilog();
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        var config = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

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
            .AddKeyedTransient<SaveArtStrategy, WmfSaveArtStrategy>(FilePickerHelper.Wmf.Key)
            .AddSingleton<ISaveArtStrategyFactory, SaveArtStrategyFactory>()
            .Configure<LocalSettingsOptions>(config.GetSection(nameof(LocalSettingsOptions)))
            .AddSingleton<ILocalSettingsService, LocalSettingsService>()
            .AddSingleton<IPageService, PageService>()
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<IGraphicToolCommandFactory, GraphicToolCommandFactory>()
            .AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>()
            .AddTransient<ActivationHandler<ElementTheme>, ThemeActivationHandler>()
            .AddSingleton<IActivationService, ActivationService>();

        return services;
    }

    private static IServiceCollection AddViewsAndViewModels(this IServiceCollection services)
    {
        services
            .AddTransient<AboutControlViewModel>()
            .AddTransient<SettingsPageViewModel>()
            .AddTransient<GeneralSettingsControlViewModel>()
            .AddTransient<ShellPageViewModel>()
            .AddTransient<ShellPage>()
            .AddTransient<AsciiArtPageViewModel>()
            .AddTransient<AsciiArtPage>()
            .AddTransient<CharacterPaletteItemEditDialog>();

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