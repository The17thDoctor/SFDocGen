using System.Text;

namespace SFDocGen.Model.Abstraction;

public interface IReturnsValue
{
    List<SFReturnValue> ReturnValues { get; set; }
}

public record SFReturnValue : IDocValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];

    public string ConcatTypes()
    {
        if (Types.Count == 0) return "unknown";
        return string.Join(", ", Types);
    }

    public static SFReturnValue FromData(string name, string description)
    {
        SFReturnValue returnValue = new()
        {
            Name = name,
            Description = description
        };

        return returnValue;
    }

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@return {ConcatTypes()} {(Name != string.Empty ? Name : "_")} ");
        sb.Append(Description.Replace("\n", "<br>\n---"));
        return sb.ToString();
    }
}