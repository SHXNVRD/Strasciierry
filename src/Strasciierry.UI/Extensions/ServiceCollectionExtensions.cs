using Microsoft.Extensions.DependencyInjection;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Media.SaveArtStrategies;

namespace Strasciierry.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSaveArtStrategies(this IServiceCollection services)
    {
        services
            .AddKeyedSingleton<SaveArtStrategy, BmpSaveArtStrategy>(FilePickerHelper.Bmp.Value.First())  
            .AddKeyedSingleton<SaveArtStrategy, EmfSaveArtStrategy>(FilePickerHelper.Emf.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, GifSaveArtStrategy>(FilePickerHelper.Gif.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, HeifSaveArtStrategy>(FilePickerHelper.Heif.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, IcoSaveArtStrategy>(FilePickerHelper.Ico.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, JpgSaveArtStrategy>(FilePickerHelper.Jpg.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, PngSaveArtStrategy>(FilePickerHelper.Png.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, TiffSaveArtStrategy>(FilePickerHelper.Tiff.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, TxtSaveArtStrategy>(FilePickerHelper.Txt.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, WebpSaveArtStrategy>(FilePickerHelper.Webp.Value.First())
            .AddKeyedSingleton<SaveArtStrategy, WmfSaveArtStrategy>(FilePickerHelper.Wmf.Value.First());

        return services;
    }
}