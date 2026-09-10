namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Indicates that the documentation element returns a value
/// <br/>Example: Functions, Methods
/// </summary>
public interface IReturnsValues
{
    List<SFReturnValue> ReturnValues { get; set; }
}

public record SFReturnValue : SFDocValue
{
    public List<string> Types { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitReturnValue(this);
    }
}