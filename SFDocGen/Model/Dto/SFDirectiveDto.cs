using SFDocGen.Model.Json;

namespace SFDocGen.Model.Dto;

public record SFDirectiveDto
{
    public string Description { get; set; } = string.Empty;
    public string? Usage { get; set; }
    public string? Deprecated { get; set; }
    public FancyDict<string> Param { get; set; } = new();
}
