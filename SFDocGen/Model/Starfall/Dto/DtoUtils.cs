using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Core;
using System.Text.Json;

namespace SFDocGen.Model.Starfall.Dto;

public class DtoUtils
{
    public static Realm RealmFromBools(bool server, bool client)
    {
        return server && !client ? Realm.Server :
               !server && client ? Realm.Client :
               Realm.Shared;
    }

    public static void PopulateList<T1, T2>(FancyDict<T1> dict, List<T2> list, Func<string, T1, T2> constructor, T1? defaultValue = null) where T1 : class
    {
        foreach (KeyValuePair<int, string> entry in dict.IndexMap)
        {
            if (!dict.Data.TryGetValue(entry.Value, out var data) && defaultValue == null)
            {
                throw new KeyNotFoundException($"Missing key: {entry.Value}");
            }

            T2 result = constructor(entry.Value, data ?? defaultValue!);
            list.Insert(entry.Key, result);
        }
    }

    public static void PopulateDict<T1, T2>(FancyDict<T1> srcDict, IDictionary<string, T2> dstDict, Func<string, T1, T2> constructor) where T1 : class
    {
        foreach (var entry in srcDict.Data)
        {
            dstDict[entry.Key] = constructor(entry.Key, entry.Value);
        }
    }

    public static List<SFReturnValue> MergeReturnValueData(List<string> descriptions, List<JsonElement> types)
    {
        List<SFReturnValue> list = [];
        for (int i = 0; i < int.Max(descriptions.Count, types.Count); i++)
        {
            string? desc = descriptions.ElementAtOrDefault(i);
            JsonElement typeList = types.ElementAtOrDefault(i);

            SFReturnValue returnValue = new()
            {
                Description = desc,
                Types = typeList.ValueKind != JsonValueKind.Undefined ? SanitizeTypes(Demistify(typeList)) : []
            };

            list.Add(returnValue);
        }

        return list;
    }

    public static List<SFParameter> MergeParameterData(FancyDict<string> datas, Dictionary<string, JsonElement> typesList)
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

    public static List<string> Demistify(JsonElement mystified)
    {
        if (mystified.ValueKind == JsonValueKind.String)
        {
            return [mystified.GetString()!];
        }
        else if (mystified.ValueKind == JsonValueKind.Array)
        {
            List<string> list = [];
            foreach (JsonElement element in mystified.EnumerateArray())
            {
                list.Add(element.GetString()!);
            }

            return list;
        }
        else if (mystified.ValueKind == JsonValueKind.Undefined)
        {
            return [];
        }
        else throw new InvalidCastException();
    }

    public static string SanitizeType(string type)
    {
        type = type.Replace("...", string.Empty);
        type = type.Replace("Any", "any");

        if (type == "0") { type = "..."; }

        return type;
    }

    public static List<string> SanitizeTypes(List<string> types)
    {
        return [.. types.Select(SanitizeType)];
    }
}
