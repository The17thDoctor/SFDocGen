using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFTable : DocElement, IHasRealm
{
    public Realm Realm { get; set; }
    public List<SFTableField> Fields { get; set; } = [];

    public static SFTable FromData(string name, SFTableDto dto)
    {
        SFTable table = new()
        {
            Name = name,
            Description = dto.Description,
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client)
        };

        DtoUtils.PopulateList(dto.Field, table.Fields, (name, fdto) => SFTableField.FromData(table, name, fdto));

        return table;
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
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

    public static SFTableField FromData(SFTable parent, string name, SFTableFieldDto dto)
    {
        return new()
        {
            Name = name,
            Description = dto.Desc,
            Parent = parent,
            Type = dto.Type
        };
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@field {Name} {Type}");

        if (Description != string.Empty)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "\n---"));
        }

        return sb.ToString();
    }
}