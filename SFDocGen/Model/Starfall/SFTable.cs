using SFDocGen.Model.Abstraction;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a Starfall Table (Structure)
/// </summary>
public record SFTable : SFDocValue, IHasRealm
{
    public Realm Realm { get; set; }
    public Dictionary<string, SFTableField> Fields { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitTable(this);

        foreach (var field in Fields.Values) field.Accept(visitor);
    }
}


/// <summary>
/// Represents a field within a Starfall Table (Structure)
/// </summary>
public record SFTableField : SFDocValue, IChildObject<SFTable>
{
    public string? Type { get; set; }
    public string DefaultValue { get; set; } = "nil";
    [JsonIgnore]
    public SFTable Parent { get; set; } = default!;

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitTableField(this);
    }
}