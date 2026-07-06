using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model.Starfall;
using System.Net.Mime;
using System.Reflection;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/doc/classes")]
public class ClassController(StorageManager storage) : BaseDocumentationController(storage)
{
    [Tags("Classes")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all classes.")]
    public ActionResult<Dictionary<string, SFClass>> GetAllClasses()
    {
        return Json(Documentation.Classes, SerializerOptions);
    }

    [Tags("Classes")]
    [HttpGet("{className}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClass> GetClass(string className)
    {
        var @class = Documentation.Classes.GetValueOrDefault(className);
        return @class != null ? Json(@class, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/methods")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassMethod>> GetClassMethods(string className)
    {
        var @class = Documentation.Classes.GetValueOrDefault(className);
        return @class != null ? Json(@class.Methods, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/methods/{methodName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassMethod> GetClassMethod(string className, string methodName)
    {
        SFClass? @class = Documentation.Classes.GetValueOrDefault(className);
        if (@class == null) return NotFound();

        bool success = @class.Methods.TryGetValue(methodName, out SFClassMethod? method);
        return success ? Json(method, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassField>> GetClassFields(string className)
    {
        var @class = Documentation.Classes.GetValueOrDefault(className);
        return @class != null ? Json(@class.Fields, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassField> GetClassField(string className, string fieldName)
    {
        SFClass? @class = Documentation.Classes.GetValueOrDefault(className);
        if (@class == null) return NotFound();

        bool success = @class.Fields.TryGetValue(fieldName, out SFClassField? field);
        return success ? Json(field, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/operators")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassField>> GetClassOperators(string className)
    {
        var @class = Documentation.Classes.GetValueOrDefault(className);
        return @class != null ? Json(@class.Operators, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/operators/{operatorName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassField> GetClassOperator(string className, string operatorName)
    {
        SFClass? @class = Documentation.Classes.GetValueOrDefault(className);
        if (@class == null) return NotFound();

        bool success = @class.Operators.TryGetValue(operatorName, out SFClassOperator? @operator);
        return success ? Json(@operator, SerializerOptions) : NotFound();
    }
}
