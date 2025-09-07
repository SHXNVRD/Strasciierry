using Serilog;
using Strasciierry.Core.Helpers;
using Strasciierry.UI.Controls.AsciiCanvas;
using Windows.ApplicationModel.DataTransfer;

namespace Strasciierry.UI.Helpers;

public static class ClipboardHelper
{
    public static string AsciiCanvasCellDataFormat => nameof(AsciiCanvasCell);

    public static async Task<bool> SetAsync<T>(T value, string format, int retries = 0)
    {
        var serializedValue = await Json.StringifyAsync(value);

        var package = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        package.SetData(format, serializedValue);

        while (retries >= 0)
        {
            try
            {
                Clipboard.SetContent(package);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to copy object to clipboard. {0}", e.Message);
                retries--;
            }
        }

        return false;
    }

    public static async Task<T?> GetAsync<T>(string format)
    {
        var dataPackageView = Clipboard.GetContent();
        if (!dataPackageView.Contains(format))
            return default;

        var content = await dataPackageView.GetDataAsync(format);
        if (content is null || content is not string stringifiedContent)
            return default;

        return await Json.ToObjectAsync<T>(stringifiedContent);
    }

    public static async Task<string?> GetTextAsync()
    {
        var dataPackageView = Clipboard.GetContent();
        if (!dataPackageView.Contains(StandardDataFormats.Text))
            return default;

        return await dataPackageView.GetTextAsync();
    }
}