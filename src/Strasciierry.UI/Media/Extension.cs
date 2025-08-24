using Microsoft.Extensions.DependencyInjection;
using Strasciierry.UI.Helpers;
using Strasciierry.UI.Media.SaveArtStrategies;

namespace Strasciierry.UI.Media;

public static class Extension
{
    // TODO: Добавить проверку на то доступно ли расширение для сохранения
    public static SaveArtStrategy GetSaveArtStrategy(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        
        if (string.IsNullOrEmpty(extension))
            throw new IOException();

        return App.Current.Host.Services.GetKeyedService<SaveArtStrategy>(extension)
            ?? App.Current.Host.Services.GetRequiredKeyedService<SaveArtStrategy>(FilePickerHelper.Txt.Value);
    }
}