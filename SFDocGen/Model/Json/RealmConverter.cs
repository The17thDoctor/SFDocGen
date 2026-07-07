using SFDocGen.Model.Abstraction;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Json;

public class RealmConverter : JsonConverter<Realm>
{
    public override Realm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString() ?? throw new JsonException();
        return Enum.Parse<Realm>(value);
    }

    public override void Write(Utf8JsonWriter writer, Realm value, JsonSerializerOptions options)
    {
        writer.WriteStringValueSegment(Enum.GetName(value)!, true);
    }
}
