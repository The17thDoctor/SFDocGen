using Model;
using SFDocGen.Model;
using SFDocGen.Model.Dto;
using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace SFDocGen.Services;

public class ParserService
{
    public SFDocRoot? Documentation { get; private set; }

    public const string DOCS_PATH = "Storage/docs.json";
    public const string IMPROVED_DOCS_PATH = "Storage/docs-improved.json";
    public const string IMPROVED_DOCS_SCHEMA_PATH = "Storage/docs-improved-schema.json";

    public const string LUADOC_PATH = "Storage/LuaDoc";
    public const string HOOKS_PATH = "Storage/LuaDoc";
    public const string LIBRARIES_PATH = "Storage/LuaDoc/Libraries";
    public const string CLASSES_PATH = "Storage/LuaDoc/Classes";
    public const string TABLES_PATH = "Storage/LuaDoc/Tables";

    public const string FILE_NOT_FOUND_MESSAGE = "Documentation JSON file not found!";
    public const string DESERIALIZATION_FAILED_MESSAGE = "Failed to deserialize the JSON content.";

    private readonly ILogger<ParserService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonSerializerOptions _deserializerOptions;

    public ParserService(ILogger<ParserService> logger)
    {
        _logger = logger;
        _deserializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        _deserializerOptions.Converters.Add(new FancyDictConverter());

        _serializerOptions  = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        _serializerOptions.Converters.Add(new RealmConverter());
    }

    public ModelUpdateResult UpdateModel()
    {
        _logger.LogInformation("Updating model from JSON file...");

        if (!File.Exists(DOCS_PATH))
        {
            _logger.LogError(FILE_NOT_FOUND_MESSAGE);
            return new(false, FILE_NOT_FOUND_MESSAGE);
        }

        string jsonContent = File.ReadAllText(DOCS_PATH);


        SFDocDto? dto = null;
        try
        {
            dto = JsonSerializer.Deserialize<SFDocDto>(jsonContent, _deserializerOptions);
            if (dto == null)
            {
                _logger.LogError(DESERIALIZATION_FAILED_MESSAGE);
                return new(false, DESERIALIZATION_FAILED_MESSAGE);
            }
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            _logger.LogError(ex, DESERIALIZATION_FAILED_MESSAGE);
            return new(false, DESERIALIZATION_FAILED_MESSAGE);
        }

        SFDocRoot doc = new();
        DtoUtils.PopulateList(dto.Hooks, doc.Hooks, SFHook.FromData);
        DtoUtils.PopulateList(dto.Libraries, doc.Libraries, SFLibrary.FromData);
        DtoUtils.PopulateList(dto.Tables, doc.Tables, SFTable.FromData);
        DtoUtils.PopulateList(dto.Classes, doc.Classes, SFClass.FromData);
        DtoUtils.PopulateList(dto.Directives, doc.Directives, SFDirective.FromData);

        Documentation = doc;
        using FileStream stream = File.OpenWrite(IMPROVED_DOCS_PATH);
        JsonSerializer.Serialize(stream, doc, _serializerOptions);
        _logger.LogInformation("Model updated.");

        using FileStream schemaStream = File.OpenWrite(IMPROVED_DOCS_SCHEMA_PATH);
        using Utf8JsonWriter writer = new(schemaStream, new() { Indented = true });
        JsonNode schema = _serializerOptions.GetJsonSchemaAsNode(typeof(SFDocRoot));
        schema.WriteTo(writer, _serializerOptions);
        _logger.LogInformation("Schema updated.");

        GenerateLuaDoc();

        return new(true, string.Empty);
    }

    public void GenerateLuaDoc()
    {
        _logger.LogInformation("Generating lua documentation...");
        if (Documentation == null)
        {
            _logger.LogWarning("Documentation model not found!");
            return;
        }

        if (Directory.Exists(LUADOC_PATH)) Directory.Delete(LUADOC_PATH, recursive: true);

        Directory.CreateDirectory(LUADOC_PATH);
        Directory.CreateDirectory(HOOKS_PATH);
        Directory.CreateDirectory(LIBRARIES_PATH);
        Directory.CreateDirectory(CLASSES_PATH);
        Directory.CreateDirectory(TABLES_PATH);

        foreach (var library in Documentation.Libraries)
        {
            string path = Path.Combine(LIBRARIES_PATH, library.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {library.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(library.ToLuaDoc());
        }

        foreach (var luaClass in Documentation.Classes)
        {
            string path = Path.Combine(CLASSES_PATH, luaClass.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {luaClass.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(luaClass.ToLuaDoc());
        }

        foreach (var table in Documentation.Tables)
        {
            string path = Path.Combine(TABLES_PATH, table.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {table.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(table.ToLuaDoc());
        }
    }

    protected void AddDiagnostic(TextWriter writer, params string[] names)
    {
        writer.WriteLine($"---@diagnostic disable: {string.Join(", ", names)}");
    }
}

public readonly struct ModelUpdateResult(bool success, string reason)
{
    public bool Success { get; } = success;
    public string Reason { get; } = reason;
}