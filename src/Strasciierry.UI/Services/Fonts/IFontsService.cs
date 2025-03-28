using System.Collections.ObjectModel;

namespace Strasciierry.UI.Services.Fonts;
public interface IFontsService
{
    Collection<FontInfo> Fonts { get; }
    bool ShowMonospacedFonstOnly { get; }

    Task InitializeAsync();
    Task SetShowMonospacedFontsOnly(bool value);
}