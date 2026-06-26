using SFDocGen.Model;
using SFDocGen.Model.Abstraction;

namespace Model;

public record SFDocRoot
{
    public Dictionary<string, SFHook> Hooks { get; set; } = [];
    public Dictionary<string, SFLibrary> Libraries { get; set; } = [];
    public Dictionary<string, SFTable> Tables { get; set; } = [];
    public Dictionary<string, SFClass> Classes { get; set; } = [];
    public Dictionary<string, SFDirective> Directives { get; set; } = [];
    public DocValue? this[string property] => GetType().GetProperty(property)?.GetValue(this) as DocValue;
}

