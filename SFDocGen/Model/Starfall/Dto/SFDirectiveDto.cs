using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Core;

namespace SFDocGen.Model.Starfall.Dto;

public record SFDirectiveDto
{
    public string Description { get; set; } = string.Empty;
    public string? Usage { get; set; }
    public string? Deprecated { get; set; }
    public FancyDict<string> Param { get; set; } = new();

    public SFDirective Convert(string name)
    {
        SFDirective directive = new()
        {
            Name = name,
            Description = Description,
            Deprecated = Deprecated,
            Usage = Usage
        };

        DtoUtils.PopulateList(Param, directive.Parameters, (name, description) => new SFParameter() {
            Name = name,
            Description = description
        });

        return directive;
    }
}
