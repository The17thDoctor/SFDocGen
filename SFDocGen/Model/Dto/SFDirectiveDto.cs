using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Json;

namespace SFDocGen.Model.Dto;

public record SFDirectiveDto
{
    public string Description { get; set; } = string.Empty;
    public string? Usage { get; set; }
    public string? Deprecated { get; set; }
    public FancyDict<string> Param { get; set; } = new();

    public SFDirective FromData(string name)
    {
        SFDirective directive = new()
        {
            Name = name,
            Description = Description,
            Deprecated = Deprecated,
            Usage = Usage
        };

        DtoUtils.PopulateList(Param, directive.Parameters, SFParameter.FromData);

        return directive;
    }
}
