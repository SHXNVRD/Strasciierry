using System.Collections.ObjectModel;
using System.Drawing;

namespace Strasciierry.UI.Services.Fonts;
public interface IFontsService
{
    Collection<FontInfo> Fonts { get; }
    bool ShowMonospacedFonstOnly { get; }

    Task InitializeAsync();
    Task SetShowMonospacedFontsOnly(bool value);
}