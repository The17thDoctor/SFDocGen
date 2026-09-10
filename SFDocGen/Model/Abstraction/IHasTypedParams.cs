namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Indicates that the documentation element has typed parameters.
/// <br/>Example: Functions, Hooks
/// </summary>
public interface IHasTypedParams
{
    /// <summary>
    /// The list of parameters (order matters)
    /// </summary>
    List<SFParameter> Parameters { get; set; }
}

/// <summary>
/// Represents a typed parameter.
/// </summary>
public record SFParameter : SFDocValue
{
    public List<string> Types { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitParameter(this);
    }
}