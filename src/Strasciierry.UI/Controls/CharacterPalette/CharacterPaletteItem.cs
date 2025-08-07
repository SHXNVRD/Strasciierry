using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Strasciierry.UI.Controls.CharacterPalette;

public partial class CharacterPaletteItem : ObservableObject
{
    [ObservableProperty]
    public partial char Character { get; set; }
    [ObservableProperty]
    public partial FontFamily FontFamily { get; set; } = new FontFamily("Consolas");
    [ObservableProperty]
    public partial Color Foreground { get; set; } = Color.White;
    [ObservableProperty]
    public partial Color Background { get; set; } = Color.Transparent;
    [ObservableProperty]
    public partial FontStyle FontStyle { get; set; } = FontStyle.Regular;

    public CharacterPaletteItem(char character = '*')
    {
        Character = character;
    }

    public void Update(CharacterPaletteItem item)
    {
        Character = item.Character;
        FontFamily = new FontFamily(item.FontFamily.Name);
        Foreground = item.Foreground;
        Background = item.Background;
        FontStyle = item.FontStyle;
    }

    public CharacterPaletteItem Clone()
    {
        return new CharacterPaletteItem
        {
            Character = Character,
            FontFamily = new FontFamily(FontFamily.Name),
            Foreground = Foreground,
            Background = Background,
            FontStyle = FontStyle
        };
    }
}

public class CharacterPaletteItemValueComparer : EqualityComparer<CharacterPaletteItem>
{
    public override bool Equals(CharacterPaletteItem? x, CharacterPaletteItem? y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return x.Character.Equals(y.Character)
            && EqualityComparer<FontFamily>.Default.Equals(x.FontFamily, y.FontFamily)
            && x.Foreground.Equals(y.Foreground)
            && x.Background.Equals(y.Background)
            && x.FontStyle == y.FontStyle;
    }

    public override int GetHashCode([DisallowNull] CharacterPaletteItem obj)
    {
        return HashCode.Combine(
            obj.Character,
            obj.FontFamily,
            obj.Foreground,
            obj.Background,
            obj.FontStyle);
    }
}