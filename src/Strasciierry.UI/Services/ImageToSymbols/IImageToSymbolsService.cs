using Windows.Graphics.Imaging;

namespace Strasciierry.UI.Services.ImageToSymbols;
public interface IImageToSymbolsService
{
    Task<string> ConvertAsync(SoftwareBitmap softwareBitmap, char[]? symbols = null);
    Task<string> ConvertNegativeAsync(SoftwareBitmap softwareBitmap);
}