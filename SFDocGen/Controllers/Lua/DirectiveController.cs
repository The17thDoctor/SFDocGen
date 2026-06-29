using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs/directives")]
public class DirectiveController(StorageManager storage) : BaseDocumentationController(storage)
{
    [Tags("Directives")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all directives.")]
    public ActionResult<Dictionary<string, SFDirective>> GetAllDirectives()
    {
        return Json(Documentation.Directives, SerializerOptions);
    }

    [Tags("Directives")]
    [HttpGet("{directiveName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific directive.")]
    public ActionResult<SFDirective> GetDirective(string directiveName)
    {
        var directive = Documentation.Directives.GetValueOrDefault(directiveName);
        return directive != null ? Json(directive, SerializerOptions) : NotFound();
    }
}
