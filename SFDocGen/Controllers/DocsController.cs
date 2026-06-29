using Microsoft.AspNetCore.Mvc;
using Model;
using SFDocGen.Core;
using SFDocGen.Model;
using SFDocGen.Services;
using System.Net.Mime;

namespace SFDocGen.Controllers;

[Route("api/docs")]
[ApiController]
public class DocsController(IServiceProvider provider, StorageManager storage) : Controller
{
    private static DateTime LastFetch = DateTime.MinValue;
    private static readonly TimeSpan MinFetchDelay = TimeSpan.FromMinutes(10);

    private SFDocRoot Documentation => storage.Documentation;

    private readonly FetchService _fetcherService = provider.GetRequiredService<FetchService>();
    private readonly ParserService _parserService = provider.GetRequiredService<ParserService>();
    private readonly LuaGenerator _luaGenerator = provider.GetRequiredService<LuaGenerator>();

    [Tags("Documentation")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the entire documentation as a JSON object.")]
    public ActionResult<SFDocRoot> GetDocumentation()
    {
        return Json(Documentation, _parserService.SerializerOptions);
    }

    [Tags("Documentation")]
    [HttpGet("forceUpdate")]
    [EndpointSummary("Forces an update of the documentation (JSON & Lua).")]
    public async Task<ActionResult> ForceUpdate()
    {
        if (_fetcherService == null)
        {
            return Problem();
        }

        TimeSpan elapsedSinceLastFetch = (DateTime.Now - LastFetch);
        if (elapsedSinceLastFetch < MinFetchDelay)
        {
            int seconds = (MinFetchDelay - elapsedSinceLastFetch).Seconds;

            Response.Headers.RetryAfter = seconds.ToString();
            return StatusCode(StatusCodes.Status429TooManyRequests, $"Please retry in {seconds} second(s).");
        }

        LastFetch = DateTime.Now;

        // Perform update
        _fetcherService.Fetch();
        _parserService.UpdateDocumentation();
        _luaGenerator.GenerateLuaDoc();

        return Ok();
    }

    // Hooks endpoints

    [Tags("Hooks")]
    [HttpGet("hooks")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all hooks.")]
    public ActionResult<Dictionary<string, SFHook>> GetAllHook()
    {
        return Json(Documentation.Hooks, _parserService.SerializerOptions);
    }

    [Tags("Hooks")]
    [HttpGet("hooks/{hookName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific hook.")]
    public ActionResult<SFHook> GetHook(string hookName)
    {
        var hook = Documentation.Hooks.GetValueOrDefault(hookName);
        return hook != null ? Json(hook, _parserService.SerializerOptions) : NotFound();
    }

    // Libraries endpoints

    [Tags("Libraries")]
    [HttpGet("libraries")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all libraries.")]
    public ActionResult<Dictionary<string, SFLibrary>> GetAllLibraries()
    {
        return Json(Documentation.Libraries, _parserService.SerializerOptions);
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library.")]
    public ActionResult<SFLibrary> GetLibrary(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/functions")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all functions for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryFunction>> GetLibraryFunctions(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Functions, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/functions/{functionName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library function.")]
    public ActionResult<SFLibraryFunction> GetLibraryFunction(string libraryName, string functionName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var function = library.Functions.GetValueOrDefault(functionName);
        return function != null ? Json(function, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/tables")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all tables for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryTable>> GetLibraryTables(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Tables, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/tables/{tableName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library table.")]
    public ActionResult<SFLibraryTable> GetLibraryTable(string libraryName, string tableName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var table = library.Tables.GetValueOrDefault(tableName);
        return table != null ? Json(table, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all fields for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryField>> GetLibraryFields(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Fields, _parserService.SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("libraries/{libraryName}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library field.")]
    public ActionResult<SFLibraryField> GetLibraryField(string libraryName, string fieldName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        if (library == null) return NotFound();

        var field = library.Fields.GetValueOrDefault(fieldName);
        return field != null ? Json(field, _parserService.SerializerOptions) : NotFound();
    }
}
