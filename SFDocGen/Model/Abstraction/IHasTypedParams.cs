using System.Text;

namespace SFDocGen.Model.Abstraction;

public interface IHasTypedParams
{
    List<SFParameter> Parameters { get; set; }
}

public record SFParameter : DocValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];

    public string ConcatTypes()
    {
        if (Types.Count == 0) return "unknown";
        return string.Join("|", Types);
    }

    public static SFParameter FromData(string name, string description)
    {
        return new()
        {
            Name = name,
            Description = description
        };
    }

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@param {Name} {ConcatTypes()} ");
        sb.Append(Description.Replace("\n", "<br>\n---"));
        return sb.ToString();
    }
}