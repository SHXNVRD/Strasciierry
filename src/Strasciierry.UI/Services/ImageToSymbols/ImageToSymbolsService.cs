using Windows.Graphics.Imaging;
using Strasciierry.UI.ImageConverters;
using Strasciierry.UI.Services.UsersSymbols;

namespace Strasciierry.UI.Services.ImageToSymbols;

public class ImageToSymbolsService(IUserSymbolsService symbolsService) : IImageToSymbolsService
{
    public async Task<string> ConvertAsync(SoftwareBitmap softwareBitmap, char[]? symbols = null)
    {
        symbols ??= symbolsService.UsersSymbolsOn 
            ? symbolsService.UserSymbols 
            : symbolsService.DefaultSymbols;
        
        return await Task.Run(() => ImageToSymbolsConverter.Convert(softwareBitmap, symbols));
    }

    public async Task<string> ConvertNegativeAsync(SoftwareBitmap softwareBitmap)
    {
        var symbols = symbolsService.UsersSymbolsOn
            ? symbolsService.UserSymbolsNegative
            : symbolsService.DefaultSymbolsNegative;
        
        return await Task.Run(() => ImageToSymbolsConverter.Convert(softwareBitmap, symbols));
    }
}