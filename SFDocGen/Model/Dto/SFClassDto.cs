using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Dto;

public record SFClassDto
{
    public string? SuperType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Server { get; set; } = default;
    public bool Client { get; set; } = default;
    public FancyDict<SFClassFieldDto> Field { get; set; } = new();
    public FancyDict<SFClassOperatorDto> Operators { get; set; } = new();
    public FancyDict<SFClassMethodDto> Methods { get; set; } = new();

    public SFClass FromData(string name)
    {
        SFClass cl = new()
        {
            Name = name,
            Description = Description,
            Realm = DtoUtils.RealmFromBools(Server, Client),
            SuperType = SuperType
        };

        DtoUtils.PopulateList(Field, cl.Fields, (name, fdto) => fdto.FromData(cl, name));
        DtoUtils.PopulateList(Methods, cl.Methods, (name, mdto) => mdto.FromData(cl, name));
        DtoUtils.PopulateList(Operators, cl.Operators, (name, odto) => odto.FromData(cl, name));

        return cl;
    }
}

public record SFClassFieldDto
{
    public string Type { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;

    public SFClassField FromData(SFClass parent, string name)
    {
        return new()
        {
            Name = name,
            Description = Desc,
            Parent = parent,
            Type = DtoUtils.SanitizeType(Type)
        };
    }
}

public record SFClassOperatorDto
{
    public string Description { get; set; } = string.Empty;
    public string Lhs { get; set; } = string.Empty;
    public string? Rhs { get; set; } = null;
    public bool Commutative { get; set; } = true;

    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];

    public SFClassOperator FromData(SFClass parent, string name)
    {
        return new()
        {
            Name = name,
            Description = Description,
            Commutative = Commutative,
            LeftOperand = Lhs,
            RightOperand = Rhs,
            Parent = parent,
            ReturnValues = SFReturnValue.MergeData(Ret, ReturnTypes)
        };
    }
}

public record SFClassMethodDto
{
    public string Description { get; set; } = string.Empty;
    public JsonElement Ret { get; set; } = default!;
    public List<JsonElement> ReturnTypes { get; set; } = [];

    [JsonConverter(typeof(FancyDictAltConverter))]
    public FancyDict<string> Param { get; set; } = new();
    public Dictionary<string, JsonElement> ParamTypes { get; set; } = [];

    public SFClassMethod FromData(SFClass parent, string name)
    {
        return new()
        {
            Name = name,
            Parent = parent,
            Description = Description,
            Parameters = SFParameter.MergeData(Param, ParamTypes),
            ReturnValues = SFReturnValue.MergeData(Ret, ReturnTypes)
        };
    }
}