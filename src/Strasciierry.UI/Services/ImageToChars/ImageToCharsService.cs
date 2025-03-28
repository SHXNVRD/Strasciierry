using Microsoft.Extensions.Options;
using Strasciierry.UI.ImageConverters;
using Windows.Graphics.Imaging;

namespace Strasciierry.UI.Services.ImageToChars;
public class ImageToCharsService : IImageToCharsService
{
    private readonly IReadOnlyCollection<char> _defaultCharTable = ['.', ',', ';', '+', '*', '?', '%', 'S', '#', '@'];
    private readonly ImageToCharOptions _options;

    public ImageToCharsService(IOptions<ImageToCharOptions> options)
    {
        _options = options.Value;
    }

    public async Task<char[][]> ConvertAsync(SoftwareBitmap softwareBitmap, char[]? charTable = null)
    {
        var converter = new ImageToCharsConverterBuilder()
            .WithDefaults(options =>
            {
                options.CharTable = charTable ??= _options.CharTable ?? [.. _defaultCharTable];
            })
            .Build();

        return await converter.ConvertAsync(softwareBitmap);
    }

    public async Task<char[][]> ConvertNegativeAsync(SoftwareBitmap softwareBitmap, char[]? charTable = null)
    {
        var converter = new ImageToCharsConverterBuilder()
            .WithDefaults(options =>
            {
                options.CharTable = charTable ??= _options.CharTable ?? [.. _defaultCharTable];
            })
            .Build();

        return await converter.ConvertNegativeAsync(softwareBitmap);
    }
}
