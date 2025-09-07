using System.Drawing.Imaging;
using Windows.Storage;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Helpers.SaveArtStrategies;

public class EmfSaveArtStrategy : SaveArtStrategy
{
    protected async override Task SaveAsyncCore(IAsciiArtPageViewModel context, StorageFile file)
    {
        using var img = context.DrawArt();
        await using var stream = await file.OpenStreamForWriteAsync();
        await Task.Run(() => img.Save(stream, ImageFormat.Emf));
    }
}