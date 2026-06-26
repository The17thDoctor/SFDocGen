using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model;

/// <summary>
/// Represents a Starfall pre-processor directive.
/// <br/>Example: --@shared
/// </summary>
public record SFDirective : SFDocElement, IHasTypedParams
{
    public List<SFParameter> Parameters { get; set; } = [];

    public override string ToLuaDoc()
    {
        throw new NotImplementedException();
    }
}