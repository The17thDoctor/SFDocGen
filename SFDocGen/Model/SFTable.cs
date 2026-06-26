using SFDocGen.Model.Abstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFTable : DocElement, IHasRealm
{
    public Realm Realm { get; set; }
    public List<SFTableField> Fields { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine($"---@class {Name}");

        if (Fields.Count > 0)
        {
            sb.AppendJoin("\n", Fields.Select(f => f.ToLuaDoc()));
            sb.AppendLine();
        }

        sb.Append($"{Name} = {{}}");

        return sb.ToString();
    }
}

public record SFTableField : DocValue, IChildObject<SFTable>
{
    public string Type { get; set; } = string.Empty;

    [JsonIgnore]
    public SFTable Parent { get; init; } = default!;

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@field {Name} {Type}");

        if (Description != null)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "\n---"));
        }

        return sb.ToString();
    }
}