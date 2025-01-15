using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Strasciierry.UI.ImageConverters;
public class ImageToCharsConverterBuilder : IImageToCharsConverterBuilder
{
    private readonly ImageToCharsConverterSettings _settings = new();
    public IImageToCharsConverterBuilder WithCharTable(char[] chars)
    {
        _settings.CharTable = chars;
        return this;
    }
    public IImageToCharsConverterBuilder WithDefaults(Action<ImageToCharsConverterSettings>? configureOptions = null)
    {
        configureOptions?.Invoke(_settings);
        return this;
    }

    public ImageToCharsConverter Build()
    {
        return new ImageToCharsConverter(_settings);
    }
}
