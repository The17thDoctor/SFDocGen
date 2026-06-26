    using SFDocGen.Model.Json;

namespace SFDocGen.Model.Dto;

public record SFTableDto
{
    public string Description { get; set; } = string.Empty;
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public FancyDict<SFTableFieldDto> Field { get; set; } = new();

    public SFTable FromData(string name)
    {
        SFTable table = new()
        {
            Name = name,
            Description = Description,
            Realm = DtoUtils.RealmFromBools(Server, Client)
        };

        DtoUtils.PopulateList(Field, table.Fields, (name, fdto) => fdto.FromData(table, name));

        return table;
    }
}

public record SFTableFieldDto
{
    public string Type { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;

    public SFTableField FromData(SFTable parent, string name)
    {
        return new()
        {
            Name = name,
            Description = Desc,
            Parent = parent,
            Type = Type
        };
    }
}
