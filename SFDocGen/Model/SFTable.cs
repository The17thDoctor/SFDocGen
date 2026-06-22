using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFTable : IDocElement, IHasRealm
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
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
}

public record SFTableField : IDocValue, IChildObject<SFTable>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
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
}