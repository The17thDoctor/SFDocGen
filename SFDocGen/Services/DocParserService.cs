using Model;
using SFDocGen.Model;
using SFDocGen.Model.Dto;
using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace SFDocGen.Services;

public class DocParserService
{
    public SFDocRoot? Documentation { get; private set; }

    public const string DOCS_PATH = "Storage/docs.json";
    public const string IMPROVED_DOCS_PATH = "Storage/docs-improved.json";
    public const string IMPROVED_DOCS_SCHEMA_PATH = "Storage/docs-improved-schema.json";

    public const string FILE_NOT_FOUND_MESSAGE = "Documentation JSON file not found!";
    public const string DESERIALIZATION_FAILED_MESSAGE = "Failed to deserialize the JSON content.";

    private readonly ILogger<DocParserService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonSerializerOptions _deserializerOptions;

    public DocParserService(ILogger<DocParserService> logger)
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

        File.WriteAllText("test.lua", string.Join("\n\n\n", doc.Libraries.Select(l => l.ToLuaDoc())));

        return new(true, string.Empty);
    }
}

public readonly struct ModelUpdateResult(bool success, string reason)
{
    public bool Success { get; } = success;
    public string Reason { get; } = reason;
}