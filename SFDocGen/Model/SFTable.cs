using SFDocGen.Model.Abstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFTable : SFDocValue, IHasRealm
{
    public Realm Realm { get; set; }
    public Dictionary<string, SFTableField> Fields { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine($"---@class {Name}");

        sb.AppendLine($"{Name} = {{}}");

        if (Fields.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin("\n", Fields.Values.Select(f => f.ToLuaDoc()));
        }

        return sb.ToString();
    }
}

public record SFTableField : SFDocValue, IChildObject<SFTable>
{
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }

    [JsonIgnore]
    public SFTable Parent { get; set; } = default!;

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@type {Type ?? "unknown"}");

        if (Description != null)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "\n---"));
        }

        sb.AppendLine();
        sb.AppendLine($"{Parent.Name}.{Name} = {DefaultValue ?? "nil"}");

        return sb.ToString();
    }
}