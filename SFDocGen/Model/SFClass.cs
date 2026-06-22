using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFClass : IDocElement, IHasRealm
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public Realm Realm { get; set; }
    public List<SFClassField> Fields { get; set; } = [];
    public List<SFClassMethod> Methods { get; set; } = [];
    public List<SFClassOperator> Operators { get; set; } = [];

    public static SFClass FromData(string name, SFClassDto dto)
    {
        SFClass cl = new()
        {
            Name = name,
            Description = dto.Description,
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client)
        };

        DtoUtils.PopulateList(dto.Field, cl.Fields, (name, fdto) => SFClassField.FromData(cl, name, fdto));
        DtoUtils.PopulateList(dto.Methods, cl.Methods, (name, mdto) => SFClassMethod.FromData(cl, name, mdto));
        DtoUtils.PopulateList(dto.Operators, cl.Operators, (name, odto) => SFClassOperator.FromData(cl, name, odto));

        return cl;
    }
}

public record SFClassField : IDocValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;    

    public static SFClassField FromData(SFClass parent, string name, SFClassFieldDto dto)
    {
        return new()
        {
            Name = name,
            Description = dto.Desc,
            Parent = parent,
            Type = dto.Type
        };
    }
}

public record SFClassMethod : IDocElement, IHasTypedParams, IReturnsValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public static SFClassMethod FromData(SFClass parent, string name, SFClassMethodDto dto)
    {
        SFClassMethod method = new()
        {
            Name = name,
            Parent = parent,
            Description = dto.Description
        };

        DtoUtils.PopulateList(dto.Param, method.Parameters, SFParameter.FromData, string.Empty);
        foreach (SFParameter param in method.Parameters)
        {
            if (dto.ParamTypes.TryGetValue(param.Name, out JsonElement value))
            {
                param.Types = DtoUtils.Demistify(value);
            }
        }

        foreach (var (First, Second) in DtoUtils.Demistify(dto.Ret).Zip(dto.ReturnTypes))
        {
            method.ReturnValues.Add(new()
            {
                Description = First,
                Types = DtoUtils.Demistify(Second)
            });
        }

        return method;
    }
}

public record SFClassOperator : IDocElement, IReturnsValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public string LeftOperand { get; set; } = string.Empty;
    public string? RightOperand { get; set; }
    public bool Commutative { get; set; }

    public static SFClassOperator FromData(SFClass parent, string name, SFClassOperatorDto dto)
    {
        SFClassOperator op = new()
        {
            Name = name,
            Description = dto.Description,
            Commutative = dto.Commutative,
            LeftOperand = dto.Lhs,
            RightOperand = dto.Rhs,
            Parent = parent
        };

        foreach (var (First, Second) in DtoUtils.Demistify(dto.Ret).Zip(dto.ReturnTypes))
        {
            op.ReturnValues.Add(new()
            {
                Description = First,
                Types = DtoUtils.Demistify(Second)
            });
        }

        return op;
    }
}