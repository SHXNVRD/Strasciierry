using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Strasciierry.UI.Controls;

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
        FontFamily = item.FontFamily;
        Foreground = item.Foreground;
        Background = item.Background;
        FontStyle = item.FontStyle;
    }

    public CharacterPaletteItem Clone()
    {
        return new CharacterPaletteItem
        {
            Character = Character,
            FontFamily = FontFamily,
            Foreground = Foreground,
            Background = Background,
            FontStyle = FontStyle
        };
    }
}