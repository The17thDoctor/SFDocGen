using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Abstraction;

public abstract record SFFunction<T> : SFDocValue, IHasTypedParams, IHasRealm, IReturnsValue, ICanBeGeneric, IChildObject<T> where T : SFDocValue
{
    [JsonIgnore]
    public T Parent { get; set; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;
    public List<string> GenericTypes { get; set; } = [];
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public List<SFFunctionOverload> Overloads { get; set; } = [];

    protected abstract string GetParentDelimiter();

    public string GetSignature(bool omitParameters = false)
    {
        StringBuilder sb = new();

        sb.Append(Parent.DocName ?? Parent.Name);
        sb.Append(GetParentDelimiter());
        sb.Append(DocName ?? Name);
        sb.Append('(');
        
        if (omitParameters)
        {
            sb.Append(')');
            return sb.ToString();
        }

        sb.AppendJoin(", ", Parameters.Select(p => $"{p.Name}: {p.ConcatTypes()}"));
        sb.Append(')');

        if (ReturnValues.Count > 0)
        {
            sb.Append(": ");
            sb.AppendJoin(", ", ReturnValues.Select(r => r.ConcatTypes()));
        }

        return sb.ToString();
    }
}

public record SFFunctionOverload : IHasTypedParams, IReturnsValue
{
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append("---@overload fun(");
        sb.AppendJoin(", ", Parameters.Select(p => $"{p.Name}: {p.ConcatTypes()}"));
        sb.Append(')');

        if (ReturnValues.Count > 0)
        {
            sb.Append(": ");
            sb.AppendJoin(", ", ReturnValues.Select(r => r.ConcatTypes()));
        }

        return sb.ToString();
    }
}
