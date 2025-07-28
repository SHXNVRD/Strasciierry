using Windows.UI.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Pages.ImageConverter;

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
}
