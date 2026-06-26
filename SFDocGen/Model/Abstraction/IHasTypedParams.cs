using SFDocGen.Model.Dto;
using SFDocGen.Model.Json;
using System.Text;
using System.Text.Json;

namespace SFDocGen.Model.Abstraction;

public interface IHasTypedParams
{
    List<SFParameter> Parameters { get; set; }
}

public record SFParameter : DocValue
{
    public List<string> Types { get; set; } = [];

    public string ConcatTypes()
    {
        if (Types.Count == 0) return "unknown";
        return string.Join("|", Types);
    }

    public static SFParameter FromData(string name, string description)
    {
        return new()
        {
            Name = name,
            Description = description
        };
    }

    public static List<SFParameter> MergeData(FancyDict<string> datas, Dictionary<string, JsonElement> typesList)
    {
        List<SFParameter> result = [];

        foreach (var entry in datas.IndexMap)
        {
            string? desc = datas.Data.GetValueOrDefault(entry.Value);
            JsonElement elem = typesList.GetValueOrDefault(entry.Value);

            SFParameter param = new()
            {
                Name = entry.Value,
                Description = desc,
                Types = DtoUtils.SanitizeTypes(DtoUtils.Demistify(elem))
            };

            result.Insert(entry.Key, param);
        }

        return result;
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@param {Name} {ConcatTypes()} ");
        if (Description != null) sb.Append(Description.Replace("\n", "<br>\n---"));
        return sb.ToString();
    }
}