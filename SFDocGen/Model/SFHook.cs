using SFDocGen.Model.Abstraction;
using System.Text;

namespace SFDocGen.Model;

/// <summary>
/// Represents a starfall hook.
/// <br/>Example: render, net, think
/// </summary>
public record SFHook : SFDocElement, IHasRealm, IHasTypedParams, IReturnsValue
{
    public Realm Realm { get; set; }
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@overload fun(hookName: \"{Name}\", name: string, callback?: fun(");
        sb.AppendJoin(", ", Parameters.Select(p => $"{p.Name}: {p.ConcatTypes()}"));
        sb.Append(')');

        if (ReturnValues.Count > 0)
        {
            sb.Append(": ");
            sb.AppendJoin(", ", ReturnValues.Select(rv => rv.ConcatTypes()));
        }

        sb.Append(')');

        if (Description != null)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "<br>"));
        }

        return sb.ToString();
    }
}
