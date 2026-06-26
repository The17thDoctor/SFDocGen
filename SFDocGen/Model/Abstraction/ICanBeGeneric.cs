namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Indicates the element can accept generic arguments / return types.
/// <br/>Example: Functions, Methods
/// </summary>
public interface ICanBeGeneric
{
    List<string> GenericTypes { get; set; }
}
