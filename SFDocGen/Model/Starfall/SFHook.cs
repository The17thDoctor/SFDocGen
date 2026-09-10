using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a starfall hook.
/// <br/>Example: render, net, think
/// </summary>
public record SFHook : SFDocValue, IHasRealm, IHasTypedParams, IReturnsValues
{
    public Realm Realm { get; set; }
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitHook(this);
    }
}
