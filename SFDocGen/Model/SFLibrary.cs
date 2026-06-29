using SFDocGen.Model.Abstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFLibrary : SFDocElement, IHasRealm
{
    public string? DocName { get; set; }
    public Realm Realm { get; set; } = Realm.Shared;
    public Dictionary<string, SFLibraryFunction> Functions { get; set; } = [];
    public Dictionary<string, SFLibraryField> Fields { get; set; } = [];
    public Dictionary<string, SFLibraryTable> Tables { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.Append($"{DocName ?? Name} = {{");

        if (Fields.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin(",\n\n", Fields.Values.OrderBy(f => f.Name).Select(f => "\t" + f.ToLuaDoc().Replace("\n", "\n\t")));
            if (Tables.Count > 0) sb.Append(',');
            sb.AppendLine();
        }

        if (Tables.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin(",\n\n", Tables.Values.OrderBy(t => t.Name).Select(t => "\t" + t.ToLuaDoc().Replace("\n", "\n\t")));
            sb.AppendLine();
        }

        sb.AppendLine("}\n");
        sb.AppendJoin("\n\n", Functions.Values.OrderBy(f => f.Name).Select(f => f.ToLuaDoc()));

        return sb.ToString();
    }
}


public record SFLibraryFunction: SFDocElement, IHasRealm, IHasTypedParams, IReturnsValue, ICanBeGeneric, IChildObject<SFLibrary>
{
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;
    public List<string> GenericTypes { get; set; } = [];
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));

        if (Usage != null)
        {
            sb.AppendLine("---");
            sb.AppendLine("---Usage:");
            sb.AppendLine("---```lua");
            sb.AppendLine("---" + Usage.Replace("\n", "\n---"));
            sb.AppendLine("---```");
        }

        if (Deprecated != null)
        {
            sb.AppendLine("---@deprecated " + Deprecated.Replace("\n", "<br>\n---"));
        }

        if (GenericTypes.Count > 0)
        {
            sb.Append("---@generic ");
            sb.AppendJoin(", ", GenericTypes);
            sb.AppendLine();
        }

        if (Parameters.Count > 0)
        {
            sb.AppendJoin("\n", Parameters.Select(p => p.ToLuaDoc()));
            sb.AppendLine();
        }

        if (ReturnValues.Count > 0)
        {
            sb.AppendJoin("\n", ReturnValues.Select(rv => rv.ToLuaDoc()));
            sb.AppendLine();
        }
        
        sb.Append($"function {Parent.DocName ?? Parent.Name}.{Name}(");
        sb.AppendJoin(", ", Parameters.Select(p => p.Name));
        sb.AppendLine(") end");

        return sb.ToString();
    }
}

public record SFLibraryField : SFDocValue, IChildObject<SFLibrary>
{
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;
    public string Type { get; set; } = "unknown";
    public string Value { get; set; } = "nil";

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine("---@type " + Type);
        sb.Append($"{Name} = {Value}");
        return sb.ToString();
    }}

public record SFLibraryTable : SFDocValue, IChildObject<SFLibrary>
{
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.Append($"{Name} = {{}}");
        return sb.ToString();
    }
}