using SFDocGen.Model;
using SFDocGen.Model.Abstraction;

namespace Model;

public record SFDocRoot
{
    public List<SFHook> Hooks { get; set; } = [];
    public List<SFLibrary> Libraries { get; set; } = [];
    public List<SFTable> Tables { get; set; } = [];
    public List<SFClass> Classes { get; set; } = [];
    public List<SFDirective> Directives { get; set; } = [];
    public DocValue? this[string property] => GetType().GetProperty(property)?.GetValue(this) as DocValue;
}

