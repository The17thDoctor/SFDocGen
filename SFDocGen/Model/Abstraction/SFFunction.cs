using System.Text.Json.Serialization;

namespace SFDocGen.Model.Abstraction;

public abstract record SFFunction<T> : SFDocValue, IHasTypedParams, IHasRealm, IReturnsValues, ICanBeGeneric, IChildObject<T> where T : SFDocValue
{
    [JsonIgnore]
    public T Parent { get; set; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;
    public List<string> GenericTypes { get; set; } = [];
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public List<SFFunctionOverload> Overloads { get; set; } = [];
}

public record SFFunctionOverload : SFDocValue, IHasTypedParams, IReturnsValues
{
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public override void Accept(IDocumentationVisitor visitor)
    {
        visitor.VisitFunctionOverload(this);
    }
}
