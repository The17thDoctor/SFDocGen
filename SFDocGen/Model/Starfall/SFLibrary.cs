using SFDocGen.Model.Abstraction;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a Starfall library (a set of functions, fields & tables)
/// </summary>
public record SFLibrary : SFDocValue, IHasRealm
{
    public Realm Realm { get; set; } = Realm.Shared;
    public Dictionary<string, SFLibraryField> Fields { get; set; } = [];
    public Dictionary<string, SFLibraryFunction> Functions { get; set; } = [];
    public Dictionary<string, SFLibraryTable> Tables { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitLibrary(this);

        foreach (var field in Fields.Values) field.Accept(visitor);
        foreach (var function in Functions.Values) function.Accept(visitor);
        foreach (var table in Tables.Values) table.Accept(visitor);
    }
}


/// <summary>
/// Represents a function within a library.
/// </summary>
public record SFLibraryFunction: SFFunction<SFLibrary>
{
    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitLibraryFunction(this);

        foreach (var parameter in Parameters) parameter.Accept(visitor);
        foreach (var returnValue in ReturnValues) returnValue.Accept(visitor);
    }
}


/// <summary>
/// Represents a constant field within a library.
/// </summary>
public record SFLibraryField : SFDocValue, IChildObject<SFLibrary>, IHasRealm
{
    [JsonIgnore]
    public SFLibrary Parent { get; set; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;
    public string Type { get; set; } = "unknown";
    public string Value { get; set; } = "nil";

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitLibraryField(this);
    }
}


/// <summary>
/// Represents a table within a library.
/// </summary>
public record SFLibraryTable : SFDocValue, IChildObject<SFLibrary>, IHasRealm
{
    [JsonIgnore]
    public SFLibrary Parent { get; set; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitLibraryTable(this);
    }
}