using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs/tables")]
public class TableController(StorageManager storage) : BaseDocumentationController(storage)
{
    [Tags("Tables")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all tables.")]
    public ActionResult<Dictionary<string, SFTable>> GetAllLibraries()
    {
        return Json(Documentation.Tables, SerializerOptions);
    }

    [Tags("Tables")]
    [HttpGet("{tableName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific table.")]
    public ActionResult<SFTable> GetLibrary(string tableName)
    {
        var table = Documentation.Tables.GetValueOrDefault(tableName);
        return table != null ? Json(table, SerializerOptions) : NotFound();
    }

    [Tags("Tables")]
    [HttpGet("{tableName}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all fields for a specific table.")]
    public ActionResult<Dictionary<string, SFTableField>> GetLibraryFields(string tableName)
    {
        var table = Documentation.Tables.GetValueOrDefault(tableName);
        return table != null ? Json(table.Fields, SerializerOptions) : NotFound();
    }

    [Tags("Tables")]
    [HttpGet("{tableName}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific table field.")]
    public ActionResult<SFTableField> GetLibraryField(string tableName, string fieldName)
    {
        var table = Documentation.Tables.GetValueOrDefault(tableName);
        var field = table?.Fields.Find(f => f.Name == fieldName);

        return field != null ? Json(field, SerializerOptions) : NotFound();
    }
}
