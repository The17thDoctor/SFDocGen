using SFDocGen.Model.Abstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFTable : SFDocElement, IHasRealm
{
    public Realm Realm { get; set; }
    public List<SFTableField> Fields { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine($"---@class {Name}");

        sb.AppendLine($"{Name} = {{}}");

        if (Fields.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin("\n", Fields.Select(f => f.ToLuaDoc()));
        }

        return sb.ToString();
    }
}

public record SFTableField : SFDocValue, IChildObject<SFTable>
{
    public string Type { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = "nil";

    [JsonIgnore]
    public SFTable Parent { get; init; } = default!;

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@type {Type}");

        if (Description != null)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "\n---"));
        }

        sb.AppendLine();
        sb.AppendLine($"{Parent.Name}.{Name} = {DefaultValue}");

        return sb.ToString();
    }
}