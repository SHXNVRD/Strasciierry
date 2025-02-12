using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
        ViewModel = App.GetService<ImageConverterViewModel>();
        InitializeComponent();
    }

    private void ForegroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        ArtTextBlock.Foreground = new SolidColorBrush(sender.Color);
    }
}
