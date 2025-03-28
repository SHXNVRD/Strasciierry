using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Strasciierry.UI.ViewModels;
using Windows.UI.Text;

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

    private void BoldAppBarToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ArtTextBlock.FontWeight = 
            ArtTextBlock.FontWeight == FontWeights.Bold
            ? FontWeights.Normal
            : FontWeights.Bold;
    }

    private void ItalicAppBarToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ArtTextBlock.FontStyle =
            ArtTextBlock.FontStyle == FontStyle.Italic
            ? FontStyle.Normal
            : FontStyle.Italic;
    }

    private void UnderlineAppBarToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ArtTextBlock.TextDecorations = ArtTextBlock.TextDecorations switch
        {
            TextDecorations.None => TextDecorations.Underline,
            TextDecorations.Strikethrough => TextDecorations.Strikethrough | TextDecorations.Underline,
            TextDecorations.Underline | TextDecorations.Strikethrough => TextDecorations.Strikethrough,
            _ => TextDecorations.None
        };
    }

    private void StrikeThroughAppBarToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ArtTextBlock.TextDecorations = ArtTextBlock.TextDecorations switch
        {
            TextDecorations.None => TextDecorations.Strikethrough,
            TextDecorations.Underline => TextDecorations.Underline | TextDecorations.Strikethrough,
            TextDecorations.Underline | TextDecorations.Strikethrough => TextDecorations.Underline,
            _ => TextDecorations.None
        };
    }
}
