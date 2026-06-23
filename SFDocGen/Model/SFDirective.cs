using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;

namespace SFDocGen.Model;

public record SFDirective : IDocElement, IHasTypedParams
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public List<SFParameter> Parameters { get; set; } = [];

    public static SFDirective FromData(string name, SFDirectiveDto dto)
    {
        SFDirective directive = new()
        {
            Name = name,
            Description = dto.Description,
            Deprecated = dto.Deprecated,
            Usage = dto.Usage
        };

        DtoUtils.PopulateList(dto.Param, directive.Parameters, SFParameter.FromData);

        return directive;
    }

    public string ToLuaDoc()
    {
        throw new NotImplementedException();
    }
}