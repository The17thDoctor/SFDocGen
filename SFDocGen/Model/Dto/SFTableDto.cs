    using SFDocGen.Model.Json;

namespace SFDocGen.Model.Dto;

public record SFTableDto
{
    public string Description { get; set; } = string.Empty;
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public FancyDict<SFTableFieldDto> Field { get; set; } = new();
}

public record SFTableFieldDto
{
    public string Type { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
}
