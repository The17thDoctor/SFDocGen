using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Core;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a Starfall Table (Structure)
/// </summary>
public record SFTable : SFDocValue, IHasRealm
{
    public Realm Realm { get; set; }
    public ChildDictionary<SFTable, string, SFTableField> Fields { get; }

    public SFTable()
    {
        Fields = new ChildDictionary<SFTable, string, SFTableField>(this);
    }

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