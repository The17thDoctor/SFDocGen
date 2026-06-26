using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json;

namespace SFDocGen.Model;

public record SFHook : DocElement, IHasRealm, IHasTypedParams, IReturnsValue
{
    public Realm Realm { get; set; }
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public static SFHook FromData(string name, SFHookDto dto)
    {
        SFHook hook = new()
        {
            Name = name,
            Description = dto.Description,
            Deprecated = dto.Deprecated,
            Usage = dto.Usage,
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client),
            ReturnValues = SFReturnValue.MergeData(DtoUtils.Demistify(dto.Ret), dto.ReturnTypes)
        };

        DtoUtils.PopulateList(dto.Param, hook.Parameters, SFParameter.FromData, string.Empty);
        foreach (SFParameter param in hook.Parameters)
        {
            if (dto.ParamTypes.TryGetValue(param.Name, out JsonElement types))
            {
                param.Types = DtoUtils.SanitizeTypes(DtoUtils.Demistify(types));
            }
        }

        return hook;
    }

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

        if (Description != string.Empty)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "<br>"));
        }

        return sb.ToString();
    }
}
