using SFDocGen.Model.Abstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFLibrary : DocElement, IHasRealm
{
    public string? DocName { get; set; }
    public Realm Realm { get; set; } = Realm.Shared;
    public List<SFLibraryFunction> Functions { get; set; } = [];
    public List<SFLibraryField> Fields { get; set; } = [];
    public List<SFLibraryTable> Tables { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.Append($"{DocName ?? Name} = {{");

        if (Fields.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin(",\n\n", Fields.Select(f => "\t" + f.ToLuaDoc().Replace("\n", "\n\t")));
            if (Tables.Count > 0) sb.Append(',');
            sb.AppendLine();
        }

        if (Tables.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin(",\n\n", Tables.Select(t => "\t" + t.ToLuaDoc().Replace("\n", "\n\t")));
            sb.AppendLine();
        }

        sb.AppendLine("}\n");
        sb.AppendJoin("\n\n", Functions.Select(f => f.ToLuaDoc()));

        return sb.ToString();
    }
}


public record SFLibraryFunction: DocElement, IHasRealm, IHasTypedParams, IReturnsValue, IChildObject<SFLibrary>
{
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;
    public Realm Realm { get; set; } = Realm.Shared;
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));

        if (Usage != null)
        {
            sb.AppendLine("---");
            sb.AppendLine("---Usage:\n---```lua");
            sb.AppendLine("---" + Usage.Replace("\n", "\n---"));
            sb.AppendLine("---```");
        }

        if (Deprecated != null)
        {
            sb.AppendLine("---@deprecated" + Deprecated.Replace("\n", "<br>\n---"));
        }

        sb.AppendJoin("\n", Parameters.Select(p => p.ToLuaDoc()));
        sb.Append(Parameters.Count > 0 ? "\n" : string.Empty);
        sb.AppendJoin("\n", ReturnValues.Select(rv => rv.ToLuaDoc()));
        sb.Append(ReturnValues.Count > 0 ? "\n" : string.Empty);
        sb.Append($"function {Parent.DocName ?? Parent.Name}.{Name}(");
        sb.AppendJoin(", ", Parameters.Select(p => p.Name));
        sb.AppendLine(") end");

        return sb.ToString();
    }
}

public record SFLibraryField : DocValue, IChildObject<SFLibrary>
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

public record SFLibraryTable : DocValue, IChildObject<SFLibrary>
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