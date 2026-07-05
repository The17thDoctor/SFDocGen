using Model;
using SFDocGen.Model;
using SFDocGen.Model.Abstraction;
using System.Text.Json;

namespace SFDocGen.Core;

/// <summary>
/// Manages the files inside of the Configuration folder.
/// </summary>
public class ConfigManager
{
    private readonly string _root, _correctionsPath, _dependenciesPath;

    public ConfigManager(IConfiguration configuration)
    {
        _root = configuration.GetValue<string>("ConfigurationFolder") ?? "Configuration";
        _correctionsPath = Path.Combine(_root, "corrections.json");
        _dependenciesPath = Path.Combine(_root, "dependencies.json");
    }

    public SFDocRoot GetCorrections()
    {
        if (!File.Exists(_correctionsPath)) return new();
        SFDocRoot corrections = JsonSerializer.Deserialize<SFDocRoot>(File.ReadAllText(_correctionsPath))!;

        // Json Deserialization doesnt bind children to parents, its ugly but it works :P
        foreach (var (Parent, Child) in corrections.Classes.Values.Where(l => l != null).SelectMany(l => l.Fields.Values, Couple)) SetParent(Parent, Child);
        foreach (var (Parent, Child) in corrections.Classes.Values.Where(l => l != null).SelectMany(l => l.Operators.Values, Couple)) SetParent(Parent, Child);
        foreach (var (Parent, Child) in corrections.Classes.Values.Where(l => l != null).SelectMany(l => l.Methods.Values, Couple)) SetParent(Parent, Child);

        foreach (var (Parent, Child) in corrections.Libraries.Values.Where(l => l != null).SelectMany(l => l.Functions.Values, Couple)) SetParent(Parent, Child);
        foreach (var (Parent, Child) in corrections.Libraries.Values.Where(l => l != null).SelectMany(l => l.Tables.Values, Couple)) SetParent(Parent, Child);
        foreach (var (Parent, Child) in corrections.Libraries.Values.Where(l => l != null).SelectMany(l => l.Fields.Values, Couple)) SetParent(Parent, Child);

        foreach (var (Parent, Child) in corrections.Tables.Values.Where(l => l != null).SelectMany(l => l.Fields.Values, Couple)) SetParent(Parent, Child);

        return corrections;
    }

    private static (T1 Parent, T2 Child) Couple<T1, T2>(T1 parent, T2 child) where T1 : SFDocValue where T2 : IChildObject<T1>
    {
        return (parent, child);
    }

    private static void SetParent<T1, T2>(T1 parent, T2 child) where T1 : SFDocValue where T2 : IChildObject<T1>
    {
        child.Parent = parent;
    }

    public List<Dependency> GetDependencies()
    {
        if (!File.Exists(_dependenciesPath)) return [];
        return JsonSerializer.Deserialize<List<Dependency>>(File.ReadAllText(_dependenciesPath))!;
    }
}
