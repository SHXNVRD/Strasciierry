namespace Strasciierry.UI.ImageConverters;
public interface IImageToCharsConverterBuilder
{
    IImageToCharsConverterBuilder WithCharTable(char[] chars);
    IImageToCharsConverterBuilder WithDefaults(Action<ImageToCharsConverterSettings>? configureOptions = null);
    ImageToCharsConverter Build();
}
