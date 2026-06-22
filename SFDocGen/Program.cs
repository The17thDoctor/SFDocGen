using Model;
using SFDocGen.Model;
using SFDocGen.Model.Dto;
using SFDocGen.Model.Json;
using SFDocGen.Parser;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace SFDocGen;

public class Program
{
    public static void Main(string[] args)
    {
        var serializerOptions = StarfallDocParser.SerializerOptions;
        serializerOptions.Converters.Add(new FancyDictConverter());

        var docsJson = File.ReadAllText("./docs-formatted.json");
        var dto = StarfallDocParser.Parse(docsJson);

        SFDocRoot doc = new();
        DtoUtils.PopulateList(dto.Hooks, doc.Hooks, SFHook.FromData);
        DtoUtils.PopulateList(dto.Libraries, doc.Libraries, SFLibrary.FromData);
        DtoUtils.PopulateList(dto.Tables, doc.Tables, SFTable.FromData);
        DtoUtils.PopulateList(dto.Classes, doc.Classes, SFClass.FromData);
        DtoUtils.PopulateList(dto.Directives, doc.Directives, SFDirective.FromData);

        Console.WriteLine();

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        options.Converters.Add(new RealmConverter());

        using FileStream stream = File.OpenWrite("./docs-improved.json");
        JsonSerializer.Serialize(stream, doc, options);

        using FileStream schemaStream = File.OpenWrite("./docs-schema.json");
        using Utf8JsonWriter writer = new(schemaStream, new() { Indented = true });
        JsonNode schema = JsonSchemaExporter.GetJsonSchemaAsNode(options, typeof(SFDocRoot));
        schema.WriteTo(writer, options);

        /**
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
        **/
    }
}