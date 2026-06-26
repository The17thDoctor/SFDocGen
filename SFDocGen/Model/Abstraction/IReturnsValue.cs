using SFDocGen.Model.Dto;
using System.Text;
using System.Text.Json;

namespace SFDocGen.Model.Abstraction;

public interface IReturnsValue
{
    List<SFReturnValue> ReturnValues { get; set; }
}

public record SFReturnValue : DocValue
{
    public List<string> Types { get; set; } = [];

    public string ConcatTypes()
    {
        if (Types.Count == 0) return "unknown";
        return string.Join("|", Types);
    }

    public static SFReturnValue FromData(string name, string description)
    {
        SFReturnValue returnValue = new()
        {
            Name = name,
            Description = description
        };

        return returnValue;
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
                Description = desc ?? string.Empty,
                Types = types.ValueKind != JsonValueKind.Undefined ? DtoUtils.Demistify(types) : []
            };

            list.Add(rv);
        }

        return list;
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