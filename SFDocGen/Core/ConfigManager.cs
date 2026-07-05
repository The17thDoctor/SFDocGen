using Model;
using SFDocGen.Model;
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
        return JsonSerializer.Deserialize<SFDocRoot>(File.ReadAllText(_correctionsPath))!;
    }

    public List<Dependency> GetDependencies()
    {
        if (!File.Exists(_dependenciesPath)) return new();
        return JsonSerializer.Deserialize<List<Dependency>>(File.ReadAllText(_dependenciesPath))!;
    }
}
