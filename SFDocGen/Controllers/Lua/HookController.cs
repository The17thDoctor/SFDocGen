using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs/hooks")]
public class HookController(StorageManager storage) : BaseDocumentationController(storage)
{
    [Tags("Hooks")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns all hooks.")]
    public ActionResult<Dictionary<string, SFHook>> GetAllHook()
    {
        return Json(Documentation.Hooks, SerializerOptions);
    }

    [Tags("Hooks")]
    [HttpGet("{hookName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns informations about a specific hook.")]
    public ActionResult<SFHook> GetHook(string hookName)
    {
        var hook = Documentation.Hooks.GetValueOrDefault(hookName);
        return hook != null ? Json(hook, SerializerOptions) : NotFound();
    }
}
