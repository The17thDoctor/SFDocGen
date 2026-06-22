using Model;
using SFDocGen.Model.Dto;
using System.Text.Json;

namespace SFDocGen.Parser;

/// <summary>
/// Parses the Starfall Documentation JSON file into C# objects.
/// </summary>
public static class StarfallDocParser
{
    public static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };

    public static SFDocDto Parse(string input)
    {
        var root = JsonSerializer.Deserialize<SFDocDto>(input, SerializerOptions);
        return root!;
    }
}
