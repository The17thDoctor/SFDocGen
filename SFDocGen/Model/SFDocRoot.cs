using SFDocGen.Model;

namespace Model;

public record SFDocRoot
{
    public List<SFHook> Hooks { get; set; } = [];
    public List<SFLibrary> Libraries { get; set; } = [];
    public List<SFTable> Tables { get; set; } = [];
    public List<SFClass> Classes { get; set; } = [];
    public List<SFDirective> Directives { get; set; } = [];
}

