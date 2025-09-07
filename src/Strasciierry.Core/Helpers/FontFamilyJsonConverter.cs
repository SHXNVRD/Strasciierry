using System.Drawing;
using Newtonsoft.Json;

namespace Strasciierry.Core.Helpers;

public class FontFamilyJsonConverter : JsonConverter<FontFamily>
{
    public override FontFamily ReadJson(JsonReader reader, Type objectType, FontFamily existingValue, bool hasExistingValue, JsonSerializer serializer)
        => new((string)reader.Value);

    public override void WriteJson(JsonWriter writer, FontFamily value, JsonSerializer serializer)
        => writer.WriteValue(value.Name);
}
