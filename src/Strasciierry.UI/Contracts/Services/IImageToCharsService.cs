using Windows.Graphics.Imaging;

namespace Strasciierry.UI.Contracts.Services;
public interface IImageToCharsService
{
    Task<char[][]> ConvertAsync(SoftwareBitmap softwareBitmap, char[]? charTable = null);
    Task<char[][]> ConvertNegativeAsync(SoftwareBitmap softwareBitmap, char[]? charTable = null);
}