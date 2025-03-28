using Microsoft.UI.Xaml.Controls;


namespace Strasciierry.UI.CustomControls.ContentDialogs;

public sealed partial class ErrorContentDialog : ContentDialog
{
    public string ErrorMessage { get; set; } = String.Empty;

    public ErrorContentDialog()
    {
        InitializeComponent();
    }
}
