using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFLibrary : IDocElement, IHasRealm
{

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public string? DocName { get; set; }
    public Realm Realm { get; set; } = Realm.Shared;
    public List<SFLibraryFunction> Functions { get; set; } = [];
    public List<SFLibraryField> Fields { get; set; } = [];
    public List<SFLibraryTable> Tables { get; set; } = [];

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine($"{DocName ?? Name} = {{");
        sb.AppendJoin(",\n", Fields.Select(f => "\t" + f.ToLuaDoc().Replace("\n", "\n\t")));
        sb.AppendLine();
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendJoin("\n\n", Functions.Select(f => f.ToLuaDoc()));

        return sb.ToString();
    }

    public static SFLibrary FromData(string name, SFLibraryDto dto)
    {
        SFLibrary lib = new()
        {
            Name = name,
            Description = dto.Description,
            DocName = dto.DocName,
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client)
        };

        DtoUtils.PopulateList(dto.Fields, lib.Fields, (name, fdto) => SFLibraryField.FromData(lib, name, fdto));
        DtoUtils.PopulateList(dto.Functions, lib.Functions, (name, fdto) => SFLibraryFunction.FromData(lib, name, fdto));
        DtoUtils.PopulateList(dto.Tables, lib.Tables, (name, tdto) => SFLibraryTable.FromData(lib, name, tdto));

        return lib;
    }
}


public record SFLibraryFunction: IDocElement, IHasRealm, IHasTypedParams, IReturnsValue, IChildObject<SFLibrary>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public Realm Realm { get; set; } = Realm.Shared;
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));

        sb.AppendJoin("\n", Parameters.Select(p => p.ToLuaDoc()));
        sb.Append(Parameters.Count > 0 ? "\n" : string.Empty);
        sb.AppendJoin("\n", ReturnValues.Select(rv => rv.ToLuaDoc()));
        sb.Append(ReturnValues.Count > 0 ? "\n" : string.Empty);
        sb.Append($"function {Parent.DocName ?? Parent.Name}.{Name}(");
        sb.AppendJoin(", ", Parameters.Select(p => p.Name));
        sb.AppendLine(") end");

        return sb.ToString();
    }

    public static SFLibraryFunction FromData(SFLibrary parent, string name, SFLibraryFunctionDto dto)
    {
        SFLibraryFunction func = new()
        {
            Parent = parent,
            Name = name,
            Description = dto.Description, 
            Deprecated = dto.Deprecated,
            Usage = dto.Usage,
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client)
        };

        DtoUtils.PopulateList(dto.Param, func.Parameters, SFParameter.FromData);
        foreach (SFParameter param in func.Parameters)
        {
            // vararg params are represented like this in the docs.
            if (param.Name == "0") { param.Name = "..."; }

            if (dto.ParamTypes.TryGetValue(param.Name, out JsonElement types))
            {
                param.Types = DtoUtils.Demistify(types);
            }
        }

        foreach (var (First, Second) in DtoUtils.Demistify(dto.Ret).Zip(dto.ReturnTypes))
        {
            func.ReturnValues.Add(new()
            {
                Description = First,
                Types = DtoUtils.Demistify(Second)
            });
        }

        return func;
    }
}

public record SFLibraryField : IDocValue, IChildObject<SFLibrary>
{
    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "unknown";
    public string Value { get; set; } = "nil";

    public string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.AppendLine("---@type " + Type);
        sb.Append($"{Name} = {Value}");
        return sb.ToString();
    }

    public static SFLibraryField FromData(SFLibrary parent, string name, SFLibraryFieldDto dto)
    {
        return new()
        {
            Parent = parent,
            Name = name,
            Description = dto.Description
        };
    }
}

public record SFLibraryTable : IDocValue, IChildObject<SFLibrary>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public SFLibrary Parent { get; init; } = default!;

    public string ToLuaDoc()
    {
        throw new NotImplementedException();
    }

    public static SFLibraryTable FromData(SFLibrary parent, string name, SFLibraryTableDto dto)
    {
        return new()
        {
            Parent = parent,
            Name = name,
            Description = dto.Description
        };
    }
}