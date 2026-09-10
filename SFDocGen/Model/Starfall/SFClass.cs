using SFDocGen.Model.Abstraction;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Starfall;

/// <summary>
/// Represents a Starfall class (table with methods, operators, fields)
/// <br/>Example: Entity, Vector
/// </summary>
public record SFClass : SFDocValue, IHasRealm
{
    public string? SuperType { get; set; }
    public Realm Realm { get; set; }
    public Dictionary<string, SFClassField> Fields { get; set; } = [];
    public Dictionary<string, SFClassMethod> Methods { get; set; } = [];
    public Dictionary<string, SFClassOperator> Operators { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitClass(this);

        foreach (var field in Fields.Values) field.Accept(visitor);
        foreach (var method in Methods.Values) method.Accept(visitor);
        foreach (var @operator in Operators.Values) @operator.Accept(visitor);
    }
}


/// <summary>
/// Represents a field of a lua class.
/// </summary>
public record SFClassField : SFDocValue, IChildObject<SFClass>, IHasRealm
{
    [JsonIgnore]
    public SFClass Parent { get; set; } = default!;
    public Realm Realm { get; set; }
    public string? Type { get; set; }

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitClassField(this);
    }
}


/// <summary>
/// Represents a method of a lua class.
/// </summary>
public record SFClassMethod : SFFunction<SFClass>
{
    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitClassMethod(this);

        foreach (var parameter in Parameters) parameter.Accept(visitor);
        foreach (var returnValue in ReturnValues) returnValue.Accept(visitor);
    }
}


/// <summary>
/// Represents an operator of a lua class.
/// </summary>
public record SFClassOperator : SFDocValue, IReturnsValues, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; set; } = default!;
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public string LeftOperand { get; set; } = string.Empty;
    public string? RightOperand { get; set; }
    public bool Commutative { get; set; }

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitClassOperator(this);

        foreach (var returnValue in ReturnValues) returnValue.Accept(visitor);
    }
}