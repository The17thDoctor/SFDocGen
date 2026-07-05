using SFDocGen.Model.Abstraction;
using System.Text;

namespace SFDocGen.Model;

public record SFTypeAlias : SFDocValue
{
    public List<string> Types { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null)
        {
            sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        }

        sb.Append($"---@alias {Name} ");
        sb.AppendJoin('|', Types);

        return sb.ToString();
    }
}
