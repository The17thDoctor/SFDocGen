using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFDocGen.Model;

public record SFClass : DocElement, IHasRealm
{
    public string? SuperType { get; set; }
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
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client),
            SuperType = dto.SuperType
        };

        DtoUtils.PopulateList(dto.Field, cl.Fields, (name, fdto) => SFClassField.FromData(cl, name, fdto));
        DtoUtils.PopulateList(dto.Methods, cl.Methods, (name, mdto) => SFClassMethod.FromData(cl, name, mdto));
        DtoUtils.PopulateList(dto.Operators, cl.Operators, (name, odto) => SFClassOperator.FromData(cl, name, odto));

        return cl;
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();

        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));
        sb.Append($"---@class {Name}");
        if (SuperType != null)
        {
            sb.Append(" : " + SuperType);
        }

        sb.AppendLine();

        if (Operators.Any(o => o.IsSupported))
        {
            sb.AppendJoin("\n", Operators.Where(o => o.IsSupported).Select(o => o.ToLuaDoc()));
            sb.AppendLine();
        }

        if (Fields.Count > 0)
        {
            sb.AppendJoin("\n", Fields.Select(f => f.ToLuaDoc()));
            sb.AppendLine();
        }

        sb.Append($"{Name} = {{}}");
        sb.AppendLine();

        if (Methods.Count > 0)
        {
            sb.AppendLine();
            sb.AppendJoin("\n\n", Methods.Select(m => m.ToLuaDoc()));
        }

        return sb.ToString();
    }
}

public record SFClassField : DocValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public string Type { get; set; } = "unknown";    

    public static SFClassField FromData(SFClass parent, string name, SFClassFieldDto dto)
    {
        return new()
        {
            Name = name,
            Description = dto.Desc,
            Parent = parent,
            Type = DtoUtils.SanitizeType(dto.Type)
        };
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@field {Name} {Type}");
        
        if (Description != null)
        {
            sb.Append(' ');
            sb.Append(Description.Replace("\n", "\n---"));
        }

        return sb.ToString();
    }
}

public record SFClassMethod : DocElement, IHasTypedParams, IReturnsValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public List<SFParameter> Parameters { get; set; } = [];
    public List<SFReturnValue> ReturnValues { get; set; } = [];

    public static SFClassMethod FromData(SFClass parent, string name, SFClassMethodDto dto)
    {
        SFClassMethod method = new()
        {
            Name = name,
            Parent = parent,
            Description = dto.Description,
            ReturnValues = SFReturnValue.MergeData(DtoUtils.Demistify(dto.Ret), dto.ReturnTypes)
        };


        DtoUtils.PopulateList(dto.Param, method.Parameters, SFParameter.FromData, string.Empty);
        foreach (SFParameter param in method.Parameters)
        {
            if (param.Name != null && dto.ParamTypes.TryGetValue(param.Name, out JsonElement value))
            {
                param.Types = DtoUtils.SanitizeTypes(DtoUtils.Demistify(value));
            }
        }

        return method;
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        if (Description != null) sb.AppendLine("---" + Description.Replace("\n", "<br>\n---"));

        if (Deprecated != null)
        {
            sb.AppendLine("---@deprecated" + Deprecated.Replace("\n", "<br>\n---"));
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
   
        sb.Append($"function {Parent.Name}:{Name}(");
        sb.AppendJoin(", ", Parameters.Select(p => p.Name));
        sb.AppendLine(") end");

        return sb.ToString();
    }
}

public record SFClassOperator : DocElement, IReturnsValue, IChildObject<SFClass>
{
    [JsonIgnore]
    public SFClass Parent { get; init; } = default!;
    public List<SFReturnValue> ReturnValues { get; set; } = [];
    public string LeftOperand { get; set; } = string.Empty;
    public string? RightOperand { get; set; }
    public bool Commutative { get; set; }

    public bool IsSupported => Supported.Contains(Name?.Split("_")[0] ?? string.Empty);

    public static readonly HashSet<string> Supported = [
        "unm", "bnot", "len", "add", "sub", "mul", "div", "mod", "pow",
        "idiv", "band", "bor", "bxor", "shl", "shr", "concat", "call"
    ];

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

    protected void SingleOperand(StringBuilder sb)
    {
        sb.Append($":{ReturnValues.First().ConcatTypes()}");
    }

    protected void DoubleOperand(StringBuilder sb)
    {
        sb.Append($"({RightOperand}): {ReturnValues.First().ConcatTypes()}");
    }

    public override string ToLuaDoc()
    {
        string opName = Name?.Split('_')[0] ?? string.Empty;

        if (!Supported.Contains(opName))
        {
            Console.WriteLine($"UNSUPPORTED OPERATOR: {Parent.Name}:{Name}");
            return string.Empty;
        }

        StringBuilder sb = new();

        sb.Append($"---@operator {opName}");

        if (RightOperand != null || RightOperand == "nil")
        {
            DoubleOperand(sb);
        }
        else
        {
            SingleOperand(sb);
        }

        return sb.ToString();
    }
}