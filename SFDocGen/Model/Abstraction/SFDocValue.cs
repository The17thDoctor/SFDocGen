namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Represents the most basic value in the SF Documentation
/// <br/>Example: Parameters, Return Values
/// </summary>
public abstract record SFDocValue
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public abstract string ToLuaDoc();
}

/// <summary>
/// Represents a higher tier element within the SF documentation
/// <br/>Example: Hooks, Functions, Methods
/// </summary>
public abstract record SFDocElement : SFDocValue
{
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
}