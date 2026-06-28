using Microsoft.AspNetCore.Mvc;
using SFDocGen.Services;
using System.IO.Compression;
using System.Net.Mime;

namespace SFDocGen.Controllers;

[ApiController]
[Route("api/download")]
public class DownloadController : Controller
{
    [HttpGet("lua/full")]
    [Produces(MediaTypeNames.Application.Zip)]
    [EndpointSummary("Returns the full, uncompressed Starfall documentation.")]
    public ActionResult FullDocumentation()
    {
        if (!Directory.Exists(LuaGenerator.LUADOC_PATH))
        {
            return NotFound();
        }

        MemoryStream stream = new();
        ZipFile.CreateFromDirectory(LuaGenerator.LUADOC_PATH, stream);

        stream.Position = 0;

        return File(stream, MediaTypeNames.Application.Zip);
    }

    [HttpGet("lua/min")]
    [Produces("text/x-lua")]
    [EndpointSummary("Returns the minified Starfall documentation.")]
    public ActionResult MinifiedDocumentation()
    {
        if (!System.IO.File.Exists(LuaGenerator.MINDOC_PATH))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(LuaGenerator.MINDOC_PATH);
        return File(file, "text/x-lua");
    }

    [HttpGet("json/full")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the documentation as a JSON file.")]
    public ActionResult JsonDocumentation()
    {
        if (!System.IO.File.Exists(ParserService.IMPROVED_DOCS_PATH))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(ParserService.IMPROVED_DOCS_PATH);
        return File(file, MediaTypeNames.Application.Json);
    }

    [HttpGet("json/schema")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the documentation's JSON schema.")]
    public ActionResult JsonSchema()
    {
        if (!System.IO.File.Exists(ParserService.IMPROVED_DOCS_SCHEMA_PATH))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(ParserService.IMPROVED_DOCS_SCHEMA_PATH);
        return File(file, MediaTypeNames.Application.Json);
    }
}
