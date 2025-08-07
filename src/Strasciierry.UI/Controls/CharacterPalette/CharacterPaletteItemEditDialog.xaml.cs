using System.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Controls.CharacterPalette;

public sealed partial class CharacterPaletteItemEditDialog : ContentDialog
{
    private CharacterPaletteItem _editingItem;

    public CharacterPaletteItem EditingItem
    {
        get => _editingItem;
        set 
        {
            _editingItem = value.Clone();
            _originItem = value;
        }
    }

    private CharacterPaletteItem _originItem;

    public CharacterPaletteItemEditDialog()
    {
        InitializeComponent();
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ApplyChanges();
    }

    private void ApplyChanges()
        => _originItem.Update(EditingItem);

    private void OnStrikeoutToggleClick(object sender, RoutedEventArgs e)
    {
        AddOrRemoveFontStyle(System.Drawing.FontStyle.Strikeout);
    }

    private void OnUnderlineToggleClick(object sender, RoutedEventArgs e)
    {
        AddOrRemoveFontStyle(System.Drawing.FontStyle.Underline);
    }

    private void OnItalicToggleClick(object sender, RoutedEventArgs e)
    {
        AddOrRemoveFontStyle(System.Drawing.FontStyle.Italic);
    }

    private void OnBoldToggleClick(object sender, RoutedEventArgs e)
    {
        AddOrRemoveFontStyle(System.Drawing.FontStyle.Bold);
    }

    private void AddOrRemoveFontStyle(FontStyle fontStyle)
    {
        EditingItem.FontStyle = EditingItem.FontStyle.HasFlag(fontStyle)
            ? EditingItem.FontStyle &= ~fontStyle
            : EditingItem.FontStyle |= fontStyle;
    }
}