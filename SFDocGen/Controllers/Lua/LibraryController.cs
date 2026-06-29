using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs/libraries")]
public class LibraryController(StorageManager storage) : BaseDocumentationController(storage)
{
    [Tags("Libraries")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all libraries.")]
    public ActionResult<Dictionary<string, SFLibrary>> GetAllLibraries()
    {
        return Json(Documentation.Libraries, SerializerOptions);
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library.")]
    public ActionResult<SFLibrary> GetLibrary(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/functions")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all functions for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryFunction>> GetLibraryFunctions(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Functions, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/functions/{functionName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library function.")]
    public ActionResult<SFLibraryFunction> GetLibraryFunction(string libraryName, string functionName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        var function = library?.Functions.GetValueOrDefault(functionName);

        return function != null ? Json(function, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/tables")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all tables for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryTable>> GetLibraryTables(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Tables, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/tables/{tableName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library table.")]
    public ActionResult<SFLibraryTable> GetLibraryTable(string libraryName, string tableName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        var table = library?.Tables.GetValueOrDefault(tableName);

        return table != null ? Json(table, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all fields for a specific library.")]
    public ActionResult<Dictionary<string, SFLibraryField>> GetLibraryFields(string libraryName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        return library != null ? Json(library.Fields, SerializerOptions) : NotFound();
    }

    [Tags("Libraries")]
    [HttpGet("{libraryName}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific library field.")]
    public ActionResult<SFLibraryField> GetLibraryField(string libraryName, string fieldName)
    {
        var library = Documentation.Libraries.GetValueOrDefault(libraryName);
        var field = library?.Fields.GetValueOrDefault(fieldName);

        return field != null ? Json(field, SerializerOptions) : NotFound();
    }
}
