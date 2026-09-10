using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a type alias (---@alias in the LuaLS syntax)
/// </summary>
public record SFTypeAlias : SFDocValue
{
    public List<string> Types { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitAlias(this);
    }
}
