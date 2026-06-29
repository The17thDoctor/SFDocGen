using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using System.Net.Mime;

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
        var cl = Documentation.Classes.GetValueOrDefault(className);
        return cl != null ? Json(cl, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/methods")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassMethod>> GetClassMethods(string className)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        return cl != null ? Json(cl.Methods, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/methods/{methodName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassMethod> GetClassMethod(string className, string methodName)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        var method = cl?.Methods.Find(m => m.Name == methodName);

        return method != null ? Json(method, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/fields")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassField>> GetClassFields(string className)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        return cl != null ? Json(cl.Fields, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/fields/{fieldName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassField> GetClassField(string className, string fieldName)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        var field = cl?.Fields.Find(f => f.Name == fieldName);

        return field != null ? Json(field, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/operators")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<Dictionary<string, SFClassField>> GetClassOperators(string className)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        return cl != null ? Json(cl.Operators, SerializerOptions) : NotFound();
    }

    [Tags("Classes")]
    [HttpGet("{className}/operators/{operatorName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific class.")]
    public ActionResult<SFClassField> GetClassOperator(string className, string operatorName)
    {
        var cl = Documentation.Classes.GetValueOrDefault(className);
        var op = cl?.Operators.Find(o => o.Name == operatorName);

        return op != null ? Json(op, SerializerOptions) : NotFound();
    }
}
