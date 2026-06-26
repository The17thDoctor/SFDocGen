using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;

namespace SFDocGen.Model;

public record SFDirective : DocElement, IHasTypedParams
{
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

    public override string ToLuaDoc()
    {
        throw new NotImplementedException();
    }
}