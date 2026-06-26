using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Dto;

public record SFLibraryDto
{
    public string? Description { get; set; }
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public string? DocName { get; set; }

    public FancyDict<SFLibraryFunctionDto> Functions { get; set; } = new();
    public FancyDict<SFLibraryFieldDto> Fields { get; set; } = new();
    public FancyDict<SFLibraryTableDto> Tables { get; set; } = new();

    public SFLibrary FromData(string name)
    {
        SFLibrary lib = new()
        {
            Name = name,
            Description = Description,
            DocName = DocName,
            Realm = DtoUtils.RealmFromBools(Server, Client)
        };

        DtoUtils.PopulateDict(Functions, lib.Functions, (name, fdto) => fdto.FromData(lib, name));
        DtoUtils.PopulateDict(Fields, lib.Fields, (name, fdto) => fdto.FromData(lib, name));
        DtoUtils.PopulateDict(Tables, lib.Tables, (name, tdto) => tdto.FromData(lib, name));

        return lib;
    }
}

public record SFLibraryFunctionDto
{
    public string? Description { get; set; }
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public bool Server { get; set; }
    public bool Client { get; set; }

    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];

    [JsonConverter(typeof(FancyDictAltConverter))]
    public FancyDict<string> Param { get; set; } = new();

    public Dictionary<string, JsonElement> ParamTypes { get; set; } = [];

    public SFLibraryFunction FromData(SFLibrary parent, string name)
    {
        return new()
        {
            Parent = parent,
            Name = name,
            Description = Description,
            Deprecated = Deprecated,
            Usage = Usage,
            Realm = DtoUtils.RealmFromBools(Server, Client),
            Parameters = SFParameter.MergeData(Param, ParamTypes),
            ReturnValues = SFReturnValue.MergeData(Ret, ReturnTypes)
        };
    }
}

public record SFLibraryFieldDto
{
    public string? Description { get; set; }

    public SFLibraryField FromData(SFLibrary parent, string name)
    {
        return new()
        {
            Parent = parent,
            Name = name,
            Description = Description
        };
    }
}

public record SFLibraryTableDto
{
    public string? Description { get; set; }

    public SFLibraryTable FromData(SFLibrary parent, string name)
    {
        return new()
        {
            Parent = parent,
            Name = name,
            Description = Description
        };
    }
}