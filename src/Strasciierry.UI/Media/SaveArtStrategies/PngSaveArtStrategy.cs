using System.Drawing.Imaging;
using Windows.Storage;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Media.SaveArtStrategies;

public class PngSaveArtStrategy : SaveArtStrategy
{
    protected async override Task SaveAsyncCore(IAsciiArtPageViewModel context, StorageFile file)
    {
        using var img = context.DrawArt();
        await using var stream = await file.OpenStreamForWriteAsync();
        await Task.Run(() => img.Save(stream, ImageFormat.Png));
    }
}