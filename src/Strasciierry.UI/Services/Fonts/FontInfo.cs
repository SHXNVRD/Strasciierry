﻿using System.Drawing;

namespace Strasciierry.UI.Services.Fonts;

public class FontInfo
{
    public FontFamily FontFamily { get; init; }
    public bool IsMonospaced { get; init; }
    public IEnumerable<FontStyle> FontStyles { get; init; } = [];

    public FontInfo(FontFamily fontFamily, bool isMonospaced = false, IEnumerable<FontStyle>? fontStyles = null)
    {
        FontFamily = fontFamily;
        IsMonospaced = isMonospaced;
        FontStyles = fontStyles ?? FontStyles;
    }
}
