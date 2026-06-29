using Microsoft.AspNetCore.Mvc;
using Model;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Services;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs")]
public class DocsController(IServiceProvider provider, StorageManager storage) : BaseDocumentationController(storage)
{
    private static DateTime LastFetch = DateTime.MinValue;
    private static readonly TimeSpan MinFetchDelay = TimeSpan.FromMinutes(10);

    private readonly FetchService _fetcherService = provider.GetRequiredService<FetchService>();
    private readonly ParserService _parserService = provider.GetRequiredService<ParserService>();
    private readonly LuaGenerator _luaGenerator = provider.GetRequiredService<LuaGenerator>();

    [Tags("Documentation")]
    [HttpGet("")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the entire documentation as a JSON object.")]
    public ActionResult<SFDocRoot> GetDocumentation()
    {
        return Json(Documentation, SerializerOptions);
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
}
