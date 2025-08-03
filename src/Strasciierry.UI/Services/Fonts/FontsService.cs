using System.Drawing;
using Strasciierry.UI.Helpers;

namespace Strasciierry.UI.Services.Fonts;
public class FontsService : IFontsService
{
    private List<FontInfo> _fonts { get; set; } = [];
    private bool _initialized;

    public void Initialize()
    {
        if (!_initialized)
        {
             LoadFonts();
            _initialized = true;
        }
    }

    public IEnumerable<FontInfo> GetFonts()
    {
        if (!_initialized)
            Initialize();

        return _fonts;
    }

    public IEnumerable<FontInfo> GetMonospacedFonts()
    {
        var fonts = GetFonts();

        return fonts.Where(f => f.IsMonospaced);
    }

    private void LoadFonts()
    {
        foreach (var fontFamily in FontFamily.Families)
        {
            var isMonospaced = FontHelper.IsMonospaced(fontFamily);
            List<FontStyle> availableFontStyles = [];

            foreach (FontStyle fontStyle in Enum.GetValues(typeof(FontStyle)))
            {
                if (fontFamily.IsStyleAvailable(fontStyle))
                    availableFontStyles.Add(fontStyle);
            }

            _fonts.Add(new FontInfo(fontFamily, isMonospaced, availableFontStyles));
        }
    }
}
