using System.Text;
using Strasciierry.UI.Extensions;
using Windows.Graphics.Imaging;
using WinRT;

namespace Strasciierry.UI.ImageConverters;
public static class ImageToSymbolsConverter
{
    public static string Convert(SoftwareBitmap softwareBitmap, char[] charTable)
    {
        var bitmapHeight = softwareBitmap.PixelHeight;
        var bitmapWidth = softwareBitmap.PixelWidth;
        
        var capacity = bitmapWidth * bitmapHeight + bitmapHeight * Environment.NewLine.Length;
        var sb = new StringBuilder(capacity);

        using var buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read);
        using var reference = buffer.CreateReference();
        
        unsafe
        {
            reference
                .As<IMemoryBufferByteAccess>()
                .GetBuffer(out var pixels, out _);

            for (var y = 0; y < bitmapHeight; y++)
            {
                for (var x = 0; x < bitmapWidth; x++)
                {
                    var index = y * bitmapWidth + x;
                    var pixelValue = pixels[index];
                    var mapIndex = (int)Map(pixelValue, 0, 255, 0, charTable.Length - 1);
                    sb.Append(charTable[mapIndex]);
                }
                
                // To avoid a new line at the end of the art
                if (y < bitmapHeight - 1)
                    sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static double Map(float valueMap, float start1, float stop1, float start2, float stop2)
    {
        var mappedValue = (valueMap - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        return Math.Round(mappedValue, MidpointRounding.AwayFromZero);
    }
}
