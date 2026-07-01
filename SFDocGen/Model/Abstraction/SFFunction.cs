using System.Text;

namespace SFDocGen.Model.Abstraction;

public abstract record SFFunction : SFDocElement, IHasTypedParams, IHasRealm, IReturnsValue, ICanBeGeneric
{
    public Realm Realm { get; set; } = Realm.Shared;
    public List<string> GenericTypes { get; set; } = [];
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public List<SFFunctionOverload> Overloads { get; set; } = [];
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
