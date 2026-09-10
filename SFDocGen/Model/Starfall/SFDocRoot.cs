using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents the root of the starfall documentation.
/// </summary>
public record SFDocRoot
{
    public Dictionary<string, SFHook> Hooks { get; set; } = [];
    public Dictionary<string, SFLibrary> Libraries { get; set; } = [];
    public Dictionary<string, SFTable> Tables { get; set; } = [];
    public Dictionary<string, SFClass> Classes { get; set; } = [];
    public Dictionary<string, SFDirective> Directives { get; set; } = [];
    public Dictionary<string, SFTypeAlias> Aliases { get; set; } = [];
    public SFDocValue? this[string property] => GetType().GetProperty(property)?.GetValue(this) as SFDocValue;

    public void Accept(IDocumentationVisitor visitor)
    {
        foreach (var hook in Hooks.Values) hook.Accept(visitor);
        foreach (var library in Libraries.Values) library.Accept(visitor);
        foreach (var table in Tables.Values) table.Accept(visitor);
        foreach (var @class in Classes.Values) @class.Accept(visitor);
        foreach (var directive in Directives.Values) directive.Accept(visitor);
        foreach (var alias in Aliases.Values) alias.Accept(visitor); 
    }
}

