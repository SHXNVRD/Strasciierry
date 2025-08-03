
namespace Strasciierry.UI.Services.Fonts;
public interface IFontsService
{
    IEnumerable<FontInfo> GetFonts();
    IEnumerable<FontInfo> GetMonospacedFonts();
    void Initialize();
}