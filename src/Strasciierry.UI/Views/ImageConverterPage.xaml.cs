using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.Controls.AsciiCanvas;
using Strasciierry.UI.Controls.CharacterPalette;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Views;

public sealed partial class ImageConverterPage : Page
{
    public ImageConverterViewModel ViewModel
    {
        get;
    }

    public ImageConverterPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ImageConverterViewModel>();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AsciiCanvas.DrawingPropertiesChanged += AsciiCanvas_DrawingPropertiesChanged;
    }

    private void AsciiCanvas_DrawingPropertiesChanged(object? sender, DrawingPropertiesChangedEventArgs e)
    {
        var selectingItem = new CharacterPaletteItem
        {
            Character = e.Character,
            Foreground = e.Foreground,
            Background = e.Background,
            FontFamily = e.FontFamily,
            FontStyle = e.FontStyle
        };

        CharacterPalette.SelectOrAdd(selectingItem);
    }
}
