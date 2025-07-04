using Microsoft.UI.Xaml.Controls;

namespace Strasciierry.UI.Controls;

public sealed partial class ErrorContentDialog : ContentDialog
{
    public string ErrorMessage { get; set; } = String.Empty;

    public ErrorContentDialog()
    {
        this.InitializeComponent();
    }
}