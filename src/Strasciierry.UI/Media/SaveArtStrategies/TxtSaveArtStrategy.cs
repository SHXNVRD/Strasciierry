using Windows.Storage;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Media.SaveArtStrategies;

public class TxtSaveArtStrategy : SaveArtStrategy
{
    protected async override Task SaveAsyncCore(IAsciiArtPageViewModel context, StorageFile file)
    {
        var art = context.GetArt();
        await File.WriteAllTextAsync(file.Path, art);
    }
}