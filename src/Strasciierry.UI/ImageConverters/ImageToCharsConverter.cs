using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strasciierry.UI.Extensions;
using Windows.Graphics.Imaging;
using WinRT;

namespace Strasciierry.UI.ImageConverters;
public class ImageToCharsConverter
{
    private char[] _charTable;
    public char[] CharTable
    {
        get => _charTable;
        private set
        {
            _charTableNegative = value.Reverse().ToArray();
            _charTable = value;
        }
    }

    private char[] _charTableNegative;

    public ImageToCharsConverter(ImageToCharsConverterSettings settings)
    {
        CharTable = settings.CharTable;
    }

    public async Task<char[][]> ConvertAsync(SoftwareBitmap softwareBitmap) =>
        await Task.Run(() => Convert(softwareBitmap, CharTable));

    public async Task<char[][]> ConvertNegativeAsync(SoftwareBitmap softwareBitmap) =>
        await Task.Run(() => Convert(softwareBitmap, _charTableNegative));

    private char[][] Convert(SoftwareBitmap softwareBitmap, char[] charTable)
    {
        var result = new char[softwareBitmap.PixelHeight][];

        using (var buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
        using (var reference = buffer.CreateReference())
        {
            unsafe
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out var pixels, out var capacity);

                for (var y = 0; y < softwareBitmap.PixelHeight; y++)
                {
                    result[y] = new char[softwareBitmap.PixelWidth];

                    for (var x = 0; x < softwareBitmap.PixelWidth; x++)
                    {
                        var index = y * softwareBitmap.PixelWidth + x;
                        var pixelValue = pixels[index];
                        var mapIndex = (int)Map(pixelValue, 0, 255, 0, charTable.Length - 1);
                        result[y][x] = charTable[mapIndex];
                    }
                }
            }
        }
        return result;
    }

    private static double Map(float valueMap, float start1, float stop1, float start2, float stop2)
    {
        var mappedValue = (valueMap - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        return Math.Round(mappedValue, MidpointRounding.AwayFromZero);
    }
}
