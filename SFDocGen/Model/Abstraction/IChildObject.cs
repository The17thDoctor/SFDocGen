namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Represents a documentation object with a strong tie to its parent
/// <br/>Example: Methods, Fields
/// </summary>
/// <typeparam name="TParent">The type of the parent object.</typeparam>
public interface IChildObject<TParent>
{
    TParent Parent { get; }
}
