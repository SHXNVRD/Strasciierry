using System.Drawing;

namespace Strasciierry.UI.Helpers;
internal static class FontHelper
{
    public static bool IsMonospaced(FontFamily fontFamily)
    {
        using var bmp = new Bitmap(1, 1);
        using var g = Graphics.FromImage(bmp);
        using var font = new Font(fontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel);

        var widthI = g.MeasureString("I", font).Width;
        var widthW = g.MeasureString("W", font).Width;

        return Math.Abs(widthI - widthW) < 0.1f;
    }

}
