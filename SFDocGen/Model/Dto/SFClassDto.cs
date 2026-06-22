using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Dto;

public record SFClassDto
{
    public string? SuperType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public FancyDict<SFClassFieldDto> Field { get; set; } = new();
    public FancyDict<SFClassOperatorDto> Operators { get; set; } = new();
    public FancyDict<SFClassMethodDto> Methods { get; set; } = new();
}

public record SFClassFieldDto
{
    public string Type { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
}

public record SFClassOperatorDto
{
    public string Description { get; set; } = string.Empty;
    public string Lhs { get; set; } = string.Empty;
    public string? Rhs { get; set; } = null;
    public bool Commutative { get; set; } = true;

    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];
}

public record SFClassMethodDto
{
    public string Description { get; set; } = string.Empty;
    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];
    public FancyDict<string> Param { get; set; } = new();
    public Dictionary<string, JsonElement> ParamTypes { get; set; } = [];
}