using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a Starfall pre-processor directive.
/// <br/>Example: --@shared
/// </summary>
public record SFDirective : SFDocValue, IHasTypedParams
{
    public List<SFParameter> Parameters { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitDirective(this);
    }
}