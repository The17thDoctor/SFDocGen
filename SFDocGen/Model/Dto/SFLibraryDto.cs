using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Dto;

public record SFLibraryDto
{
    public string Description { get; set; } = string.Empty;
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public string? DocName { get; set; }

    public FancyDict<SFLibraryFunctionDto> Functions { get; set; } = new();
    public FancyDict<SFLibraryFieldDto> Fields { get; set; } = new();
    public FancyDict<SFLibraryTableDto> Tables { get; set; } = new();
}

public record SFLibraryFunctionDto
{
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public bool Server { get; set; }
    public bool Client { get; set; }

    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];

    [JsonConverter(typeof(FancyDictAltConverter))]
    public FancyDict<string> Param { get; set; } = new();

    public Dictionary<string, JsonElement> ParamTypes { get; set; } = [];
}

public record SFLibraryFieldDto
{
    public string Description { get; set; } = string.Empty;
}

public record SFLibraryTableDto
{
    public string Description { get; set; } = string.Empty;
}