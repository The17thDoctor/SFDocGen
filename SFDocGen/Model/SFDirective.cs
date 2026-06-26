using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model;

public record SFDirective : DocElement, IHasTypedParams
{
    public List<SFParameter> Parameters { get; set; } = [];

    public override string ToLuaDoc()
    {
        throw new NotImplementedException();
    }
}