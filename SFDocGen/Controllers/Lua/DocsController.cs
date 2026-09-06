using Microsoft.AspNetCore.Mvc;
using SFDocGen.Controllers.Abstraction;
using SFDocGen.Core;
using SFDocGen.Model;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;
using SFDocGen.Services.Backend;
using System.Collections;
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
    [HttpGet("search")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns a list of elements that may match the given term")]
    public ActionResult<IEnumerable<SearchResult>> Search([FromQuery] string term)
    {
        List<SearchResult> results = [];
        RecursiveSearch(ref results, Storage.Documentation, term.ToLower());
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

        TimeSpan elapsedSinceLastFetch = (DateTime.Now - LastFetch);
        if (elapsedSinceLastFetch < MinFetchDelay)
        {
            long seconds = (long)(MinFetchDelay - elapsedSinceLastFetch).TotalSeconds;

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

    private static void RecursiveSearch(ref List<SearchResult> results, object item, string searchTerm)
    {
        Type objType = item.GetType();
        foreach (var property in objType.GetProperties())
        {
            if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(Dictionary<,>)) continue;
            if (!property.PropertyType.GenericTypeArguments[1].IsAssignableTo(typeof(SFDocValue))) continue;

            IDictionary a = (IDictionary)property.GetMethod?.Invoke(item, null)!;
            foreach (DictionaryEntry entry in a)
            {
                SFDocValue value = (SFDocValue)entry.Value!;
                RecursiveSearch(ref results, value, searchTerm);
                SearchResult result = SearchResult.FromValue(value);

                if (result.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
                {
                    results.Add(result);
                }
            }
        }
    }
}