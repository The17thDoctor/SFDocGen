using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json;

namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Indicates that the documentation element returns a value
/// <br/>Example: Functions, Methods
/// </summary>
public interface IReturnsValue
{
    List<SFReturnValue> ReturnValues { get; set; }
}

public record SFReturnValue : SFDocValue
{
    public List<string> Types { get; set; } = [];

    public string ConcatTypes()
    {
        if (Types.Count == 0) return "unknown";
        return string.Join("|", Types);
    }

    public static SFReturnValue FromData(string name, string description)
    {
        return new()
        {
            Name = name,
            Description = description
        };
    }

    public static List<SFReturnValue> MergeData(List<string> descs, List<JsonElement> typesList)
    {
        List<SFReturnValue> list = [];
        for (int i = 0; i < int.Max(descs.Count, typesList.Count); i++)
        {
            string? desc = descs.ElementAtOrDefault(i);
            JsonElement types = typesList.ElementAtOrDefault(i);

            SFReturnValue rv = new()
            {
                Description = desc,
                Types = types.ValueKind != JsonValueKind.Undefined ? DtoUtils.SanitizeTypes(DtoUtils.Demistify(types)) : []
            };

            list.Add(rv);
        }

        return list;
    }

    public static List<SFReturnValue> MergeData(JsonElement descs, List<JsonElement> typesList)
    {
        return MergeData(DtoUtils.Demistify(descs), typesList);
    }

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        sb.Append($"---@return {ConcatTypes()}");

        if (Name != null) { sb.Append($" {Name}"); }
        if (Description != null) { sb.Append(" '" + Description.Replace("\n", "<br>").Replace("'", "\\'") + "'"); }
        return sb.ToString();
    }
}