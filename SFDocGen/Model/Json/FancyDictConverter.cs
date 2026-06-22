using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Json;

public class FancyDictConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(FancyDict<>))
        {
            return false;
        }

        return true;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type[] typeArguments = typeToConvert.GetGenericArguments();
        Type valueType = typeArguments[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(FancyDictConverterInner<>).MakeGenericType([valueType]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [options],
            culture: null)!;

        return converter;
    }

    private class FancyDictConverterInner<T>(JsonSerializerOptions options) : JsonConverter<FancyDict<T>>
    {
        private readonly JsonConverter<T> _valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));
        private readonly JsonConverter<T[]> _arrayValueConverter = (JsonConverter<T[]>)options.GetConverter(typeof(T[]));

        public override FancyDict<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FancyDict<T> dict = new();

            // In the case of an array, we will treat it as a list of values with integer keys.
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                T[] array = _arrayValueConverter.Read(ref reader, typeof(T[]), options)!;
                for (int i = 0; i < array.Length; i++)
                {
                    dict.IndexMap.Add(i, i.ToString());
                    dict.Data.Add(i.ToString(), array[i]);
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
                Console.WriteLine($"{new string('\t', reader.CurrentDepth)}Processing key: {key}");
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
                    T value = _valueConverter.Read(ref reader, typeof(T), options)!;
                    dict.Data[key] = value;
                }
            }

            return dict;
        }

        public override void Write(Utf8JsonWriter writer, FancyDict<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
