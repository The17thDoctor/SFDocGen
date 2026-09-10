using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;
using SFDocGen.Services.Backend;
using System.Net.Mime;

namespace SFDocGen.Controllers.Lua;

[ApiController]
[Route("api/docs")]
public class DocsController(IServiceProvider provider, StorageManager storage) : BaseDocumentationController(storage)
{
    private static DateTime LastUpdate = DateTime.MinValue;
    private static readonly TimeSpan ForceUpdateDelay = TimeSpan.FromMinutes(1);

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
    [HttpGet("search")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns a list of elements that may match the given term")]
    public ActionResult<IEnumerable<SearchResult>> Search([FromQuery] string term)
    {
        List<SFDocValue> matches = SFDocTreeFinder.Find(Documentation, value => value.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false);
        List<SearchResult> results = [.. matches.Select(SearchResult.FromValue)];

        results.Sort();

        return Json(results, SerializerOptions);
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

        TimeSpan elapsedSinceLastFetch = (DateTime.Now - LastUpdate);
        if (elapsedSinceLastFetch < ForceUpdateDelay)
        {
            long seconds = (long)(ForceUpdateDelay - elapsedSinceLastFetch).TotalSeconds;

            Response.Headers.RetryAfter = seconds.ToString();
            return StatusCode(StatusCodes.Status429TooManyRequests, $"Please retry in {seconds} second(s).");
        }

        LastUpdate = DateTime.Now;

        // Perform update
        _fetcherService.Fetch();
        _parserService.UpdateDocumentation();
        _luaGenerator.GenerateLuaDoc();

        return Ok();
    }
}