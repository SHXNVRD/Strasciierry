using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Strasciierry.UI.Helpers;

// Taken from: https://github.com/CommunityToolkit/Windows/blob/main/components/ColorPicker/src/ColorPickerRenderingHelpers.cs
public class RenderingHelper
{
    public static readonly Color CheckerBackgroundColor = Color.FromArgb(0x19, 0x80, 0x80, 0x80);

    /// <summary>
    /// Generates a new checkered bitmap of the specified size.
    /// </summary>
    /// <remarks>
    /// This is a port and heavy modification of the code here:
    /// https://github.com/microsoft/microsoft-ui-xaml/blob/865e4fcc00e8649baeaec1ba7daeca398671aa72/dev/ColorPicker/ColorHelpers.cpp#L363
    /// UWP needs TiledBrush support.
    /// </remarks>
    /// <param name="width">The pixel width (X, horizontal) of the checkered bitmap.</param>
    /// <param name="height">The pixel height (Y, vertical) of the checkered bitmap.</param>
    /// <param name="checkerColor">The color of the checker square.</param>
    /// <returns>A new checkered bitmap of the specified size.</returns>
    public static async Task<byte[]> CreateCheckeredBitmapAsync(
        int width,
        int height,
        Color checkerColor)
    {
        // The size of the checker is important. You want it big enough that the grid is clearly discernible.
        // However, the squares should be small enough they don't appear unnaturally cut at the edge of backgrounds.
        int checkerSize = 4;

        if (width == 0 || height == 0)
        {
            return null!;
        }

        var bitmap = await Task.Run<byte[]>(() =>
        {
            int pixelDataIndex = 0;
            byte[] bgraPixelData;

            // Allocate the buffer
            // BGRA formatted color channels 1 byte each (4 bytes in a pixel)
            bgraPixelData = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // We want the checkered pattern to alternate both vertically and horizontally.
                    // In order to achieve that, we'll toggle visibility of the current pixel on or off
                    // depending on both its x- and its y-position.  If x == CheckerSize, we'll turn visibility off,
                    // but then if y == CheckerSize, we'll turn it back on.
                    // The below is a shorthand for the above intent.
                    bool pixelShouldBeBlank = ((x / checkerSize) + (y / checkerSize)) % 2 == 0 ? true : false;

                    // Remember, use BGRA pixel format with pre-multiplied alpha values
                    if (pixelShouldBeBlank)
                    {
                        bgraPixelData[pixelDataIndex + 0] = 0;
                        bgraPixelData[pixelDataIndex + 1] = 0;
                        bgraPixelData[pixelDataIndex + 2] = 0;
                        bgraPixelData[pixelDataIndex + 3] = 0;
                    }
                    else
                    {
                        bgraPixelData[pixelDataIndex + 0] = Convert.ToByte(checkerColor.B * checkerColor.A / 255);
                        bgraPixelData[pixelDataIndex + 1] = Convert.ToByte(checkerColor.G * checkerColor.A / 255);
                        bgraPixelData[pixelDataIndex + 2] = Convert.ToByte(checkerColor.R * checkerColor.A / 255);
                        bgraPixelData[pixelDataIndex + 3] = checkerColor.A;
                    }

                    pixelDataIndex += 4;
                }
            }

            return bgraPixelData;
        });

        return bitmap;
    }

    /// <summary>
    /// Converts the given bitmap (in raw BGRA pre-multiplied alpha pixels) into an image brush
    /// that can be used in the UI.
    /// </summary>
    /// <param name="bitmap">The bitmap (in raw BGRA pre-multiplied alpha pixels) to convert to a brush.</param>
    /// <param name="width">The pixel width of the bitmap.</param>
    /// <param name="height">The pixel height of the bitmap.</param>
    /// <returns>A new ImageBrush.</returns>
    public static async Task<ImageBrush> BitmapToBrushAsync(
        byte[] bitmap,
        int width,
        int height)
    {
        var writableBitmap = new WriteableBitmap(width, height);
        using (Stream stream = writableBitmap.PixelBuffer.AsStream())
        {
            await stream.WriteAsync(bitmap, 0, bitmap.Length);
        }

        var brush = new ImageBrush()
        {
            ImageSource = writableBitmap,
            Stretch = Stretch.None
        };

        return brush;
    }

    /// <summary>
    /// Centralizes code to create a checker brush for a <see cref="Border"/>.
    /// </summary>
    /// <param name="border">Border which will have its Background modified.</param>
    /// <param name="color">Color to use for transparent checkerboard.</param>
    /// <returns>Task</returns>
    public static async Task UpdateBorderBackgroundWithCheckerAsync(Border border, Color color)
    {
        if (border != null)
        {
            int width = Convert.ToInt32(border.ActualWidth);
            int height = Convert.ToInt32(border.ActualHeight);

            var bitmap = await CreateCheckeredBitmapAsync(
                width,
                height,
                color);

            if (bitmap != null)
            {
                border.Background = await BitmapToBrushAsync(bitmap, width, height);
            }
        }
    }
}