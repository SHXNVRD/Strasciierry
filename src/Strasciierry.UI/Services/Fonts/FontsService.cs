using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using Strasciierry.UI.Contracts.Services;
using Strasciierry.UI.Helpers;

namespace Strasciierry.UI.Services.Fonts;
public class FontsService : IFontsService
{
    private readonly ILocalSettingsService _settingsService;
    private bool _initialized;

    public const string UseMonospacedFonstOnlySettingsKey = "ShowMonospacedFonstOnly";

    public bool ShowMonospacedFonstOnly { get; private set; }
    public Collection<FontInfo> Fonts { get; private set; } = [];

    public FontsService(ILocalSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        if (!_initialized)
        {
            ShowMonospacedFonstOnly = await _settingsService.ReadSettingAsync<bool>(UseMonospacedFonstOnlySettingsKey);
            LoadFonts();
            _initialized = true;
        }
    }

    public async Task SetShowMonospacedFontsOnly(bool value)
    {
        await _settingsService.SaveSettingAsync(UseMonospacedFonstOnlySettingsKey, value);
        ShowMonospacedFonstOnly = value;
    }

    private void LoadFonts()
    {
        foreach (var fontFamily in FontFamily.Families)
        {
            var isMonospaced = FontHelper.IsMonospaced(fontFamily);
            ICollection<FontStyle> availableFontStyles = [];

            foreach (FontStyle fontStyle in Enum.GetValues(typeof(FontStyle)))
            {
                if (fontFamily.IsStyleAvailable(fontStyle))
                    availableFontStyles.Add(fontStyle);
            }

            Fonts.Add(new FontInfo(fontFamily, isMonospaced, availableFontStyles));
        }
    }
}
