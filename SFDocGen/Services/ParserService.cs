using Model;
using SFDocGen.Core;
using SFDocGen.Model.Dto;
using SFDocGen.Model.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace SFDocGen.Services;

public class ParserService
{
    public JsonSerializerOptions SerializerOptions { get; private set; }

    public const string FILE_NOT_FOUND_MESSAGE = "Documentation JSON file not found!";
    public const string DESERIALIZATION_FAILED_MESSAGE = "Failed to deserialize the JSON content.";

    private readonly ILogger<ParserService> _logger;
    private readonly JsonSerializerOptions _deserializerOptions;

    private readonly StorageManager _storage;
    private readonly CorrecterService _correcterService;

    public ParserService(ILogger<ParserService> logger, CorrecterService correcterService, StorageManager storage)
    {
        _logger = logger;
        _storage = storage;
        _correcterService = correcterService;

        _deserializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        _deserializerOptions.Converters.Add(new FancyDictConverter());

        SerializerOptions  = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        SerializerOptions.Converters.Add(new RealmConverter());
    }

    public UpdateResult UpdateDocumentation()
    {
        _logger.LogInformation("Updating model from JSON file...");

        if (!File.Exists(StorageManager.Files.OriginalDoc))
        {
            _logger.LogError(FILE_NOT_FOUND_MESSAGE);
            return new(false, FILE_NOT_FOUND_MESSAGE);
        }

        // Convert JSON => DTO
        string jsonContent = File.ReadAllText(StorageManager.Files.OriginalDoc);
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

        // Convert DTO => Starfall Documentation
        SFDocRoot doc = new();
        DtoUtils.PopulateDict(dto.Hooks, doc.Hooks, (name, dto) => dto.FromData(name));
        DtoUtils.PopulateDict(dto.Libraries, doc.Libraries, (name, dto) => dto.FromData(name));
        DtoUtils.PopulateDict(dto.Tables, doc.Tables, (name, dto) => dto.FromData(name));
        DtoUtils.PopulateDict(dto.Classes, doc.Classes, (name, dto) => dto.FromData(name));
        DtoUtils.PopulateDict(dto.Directives, doc.Directives, (name, dto) => dto.FromData(name));

        _correcterService.ApplyCorrection(doc);
        _storage.Documentation = doc;

        // Write Starfall Documentation & Schema to file.
        using FileStream stream = File.OpenWrite(StorageManager.Files.ImprovedDoc);
        JsonSerializer.Serialize(stream, doc, SerializerOptions);
        _logger.LogInformation("Model updated.");

        using FileStream schemaStream = File.OpenWrite(StorageManager.Files.ImprovedDocSchema);
        using Utf8JsonWriter writer = new(schemaStream, new() { Indented = true });
        JsonNode schema = SerializerOptions.GetJsonSchemaAsNode(typeof(SFDocRoot));
        schema.WriteTo(writer, SerializerOptions);
        _logger.LogInformation("Schema updated.");

        return new(true, string.Empty);
    }
}

public readonly struct UpdateResult(bool success, string reason)
{
    public bool Success { get; } = success;
    public string Reason { get; } = reason;
}