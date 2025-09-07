using Microsoft.Extensions.DependencyInjection;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Helpers.SaveArtStrategies;

namespace Strasciierry.UI.Factories;

public class SaveArtStrategyFactory(IServiceProvider serviceProvider) : ISaveArtStrategyFactory
{
    public SaveArtStrategy CreateStrategy(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("File extension cannot be null or am empty string");

        var fileFormat = FilePickerHelper.GetFileFormat(extension);

        return serviceProvider.GetKeyedService<SaveArtStrategy>(fileFormat)
            ?? serviceProvider.GetRequiredKeyedService<SaveArtStrategy>(FilePickerHelper.Txt.Key);
    }
}
