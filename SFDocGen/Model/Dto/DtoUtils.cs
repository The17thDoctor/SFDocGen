using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Json;
using System.Text.Json;

namespace SFDocGen.Model.Dto;

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

    public static Dictionary<string, T2> PopulateDict<T1, T2>(FancyDict<T1> srcDict, Func<string, T1, T2> constructor, T1? defaultValue = null) where T1 : class
    {
        Dictionary<string, T2> dstDict = [];

        foreach (var entry in srcDict.Data)
        {
            dstDict[entry.Key] = constructor(entry.Key, entry.Value);
        }

        return dstDict;
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
        return [.. types.Select(t => SanitizeType(t))];
    }
}
