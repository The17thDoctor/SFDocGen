using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Json;

public class FancyDictAltConverter : JsonConverter<FancyDict<string>>
{
    private static readonly Type _valueType = typeof(string);
    private static readonly Type _arrayValueType = typeof(string[]);


    public override FancyDict<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonConverter<string> valueConverter = (JsonConverter<string>)options.GetConverter(_valueType);
        JsonConverter<string[]> arrayValueConverter = (JsonConverter<string[]>)options.GetConverter(_arrayValueType);

        FancyDict<string> dict = new();

        // In the case of an array, we will treat it as a list of values with integer keys.
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            string[] array = arrayValueConverter.Read(ref reader, _arrayValueType, options)!;
            for (int i = 0; i < array.Length; i++)
            {
                dict.IndexMap.Add(i, array[i]);
                dict.Data.Add(array[i], string.Empty);
            }

            return dict;
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject || reader.TokenType == JsonTokenType.EndArray)
            {
                return dict;
            }

            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string key = reader.GetString()!;
            reader.Read();

            if (int.TryParse(key, out int index))
            {
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException();
                }

                string value = reader.GetString()!;
                dict.IndexMap.Add(index - 1, value);
            }
            else
            {
                string value = valueConverter.Read(ref reader, _valueType, options)!;
                dict.Data[key] = value;
            }
        }

        return dict;
    }

    public override void Write(Utf8JsonWriter writer, FancyDict<string> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
