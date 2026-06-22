using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Dto;

public record SFHookDto
{
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;

    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];

    public FancyDict<string> Param { get; set; } = new();
    public Dictionary<string, JsonElement> ParamTypes { get; set; } = [];
}