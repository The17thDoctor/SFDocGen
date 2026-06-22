using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Dto;
using System.Text.Json;

namespace SFDocGen.Model;

public record SFHook : IDocElement, IHasRealm, IHasTypedParams, IReturnsValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
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
            Realm = DtoUtils.RealmFromBools(dto.Server, dto.Client)
        };

        DtoUtils.PopulateList(dto.Param, hook.Parameters, SFParameter.FromData, string.Empty);
        foreach (SFParameter param in hook.Parameters)
        {
            if (dto.ParamTypes.TryGetValue(param.Name, out JsonElement types))
            {
                param.Types = DtoUtils.Demistify(types);
            }
        }

        foreach (var (First, Second) in DtoUtils.Demistify(dto.Ret).Zip(dto.ReturnTypes))
        {
            hook.ReturnValues.Add(new()
            {
                Description = First,
                Types = DtoUtils.Demistify(Second)
            });
        }

        return hook;
    }
}
