using Microsoft.AspNetCore.Mvc;
using Model;
using SFDocGen.Model;
using SFDocGen.Services;
using System.Net.Mime;

namespace SFDocGen.Controllers;

[Route("api/docs")]
[ApiController]
public class DocsController(ParserService parser) : Controller
{
    private SFDocRoot? Documentation => parser.Documentation;

    [Tags("Documentation")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the entire documentation as a JSON object.")]
    public ActionResult<SFDocRoot> GetDocumentation()
    {
        return Documentation != null ? Json(Documentation, parser.SerializerOptions) : NotFound();
    }

    // Hooks endpoints

    [Tags("Hooks")]
    [HttpGet("hooks")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all hooks.")]
    public ActionResult<Dictionary<string, SFHook>> GetAllHook()
    {
        return Documentation?.Hooks != null ? Json(Documentation.Hooks, parser.SerializerOptions) : NotFound();
    }

    [Tags("Hooks")]
    [HttpGet("hooks/{hookName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific hook.")]
    public ActionResult<SFHook> GetHook(string hookName)
    {
        var hook = Documentation?.Hooks.GetValueOrDefault(hookName);
        return hook != null ? Json(hook, parser.SerializerOptions) : NotFound();
    }

    // Libraries endpoints

    [Tags("Libraries")]
    [HttpGet("libraries")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all libraries.")]
    public ActionResult<Dictionary<string, SFLibrary>> GetAllLibraries()
    {
        return Documentation?.Libraries != null ? Json(Documentation.Libraries, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library.")]
    public ActionResult<SFLibrary> GetLibrary(string libraryName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/functions")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all functions for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryFunction>> GetLibraryFunctions(string libraryName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Functions, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/functions/{functionName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library function.")]
    public ActionResult<SFLibraryFunction> GetLibraryFunction(string libraryName, string functionName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var function = library.Functions.GetValueOrDefault(functionName);
        return function != null ? Json(function, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/tables")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all tables for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryTable>> GetLibraryTables(string libraryName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Tables, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/tables/{tableName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library table.")]
    public ActionResult<SFLibraryTable> GetLibraryTable(string libraryName, string tableName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var table = library.Tables.GetValueOrDefault(tableName);
        return table != null ? Json(table, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all fields for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryField>> GetLibraryFields(string libraryName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Fields, parser.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library field.")]
    public ActionResult<SFLibraryField> GetLibraryField(string libraryName, string fieldName)
    {
        var library = Documentation?.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var field = library.Fields.GetValueOrDefault(fieldName);
        return field != null ? Json(field, parser.SerializerOptions) : NotFound();
    }
}
