using Windows.Storage;
using Strasciierry.UI.ViewModels;

namespace Strasciierry.UI.Helpers.SaveArtStrategies;

public abstract class SaveArtStrategy
{
    public async Task SaveAsync(IAsciiArtPageViewModel context, StorageFile file)
    {
        ValidateArguments(context, file);
        await SaveAsyncCore(context, file);
    }

    protected void ValidateArguments(IAsciiArtPageViewModel context, StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        if (file == null)
            throw new ArgumentNullException(nameof(file));
    }

    protected abstract Task SaveAsyncCore(IAsciiArtPageViewModel context, StorageFile file);
}