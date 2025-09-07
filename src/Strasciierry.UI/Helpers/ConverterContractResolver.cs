using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Strasciierry.UI.Helpers;

public class ConverterContractResolver : DefaultContractResolver
{
    public new static readonly ConverterContractResolver Instance = new();

    protected override JsonContract CreateContract(Type objectType)
    {
        var contract = base.CreateContract(objectType);

        var converter = App.Current.Host.Services.GetKeyedService<JsonConverter>(objectType);

        if (converter is not null)
            contract.Converter = converter;

        return contract;
    }
}
