namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Represents the most basic value in the SF Documentation
/// <br/>Example: Parameters, Return Values
/// </summary>
public abstract record SFDocValue
{
    public string? Name { get; set; }
    public string? DocName { get; set; }
    public string? Description { get; set; }
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }

    /// <summary>
    /// Allows a visitor to explore the current value.
    /// </summary>
    public abstract void Accept(IDocumentationVisitor visitor);
}