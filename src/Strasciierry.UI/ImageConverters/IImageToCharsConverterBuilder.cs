using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strasciierry.UI.ImageConverters;
public interface IImageToCharsConverterBuilder
{
    IImageToCharsConverterBuilder WithCharTable(char[] chars);
    IImageToCharsConverterBuilder WithDefaults(Action<ImageToCharsConverterSettings>? configureOptions = null);
    ImageToCharsConverter Build();
}
