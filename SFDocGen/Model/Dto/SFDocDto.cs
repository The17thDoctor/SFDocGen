using SFDocGen.Model.Json;

namespace SFDocGen.Model.Dto;

public record SFDocDto
{
    public FancyDict<SFHookDto> Hooks { get; set; } = new();
    public FancyDict<SFLibraryDto> Libraries { get; set; } = new();
    public FancyDict<SFClassDto> Classes { get; set; } = new();
    public FancyDict<SFTableDto> Tables { get; set; } = new();
    public FancyDict<SFDirectiveDto> Directives { get; set; } = new();
}