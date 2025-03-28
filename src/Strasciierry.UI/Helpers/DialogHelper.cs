using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Strasciierry.UI.CustomControls.ContentDialogs;

namespace Strasciierry.UI.Helpers;
public static class DialogHelper
{
    public static async Task<ContentDialogResult> ShowAsync(
        XamlRoot root, 
        string title, 
        string message, 
        string primaryButtonText, 
        string? cancelButtonText = null, 
        string? secondaryButtonText = null)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = cancelButtonText,
            XamlRoot = root,
            DefaultButton = ContentDialogButton.Primary,
        };
        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowErrorAsync(
        XamlRoot root,
        string message)
    {
        var dialog = new ErrorContentDialog()
        {
            XamlRoot = root,
            ErrorMessage = message
        };

        return await dialog.ShowAsync();
    }
}
