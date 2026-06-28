using Microsoft.AspNetCore.Mvc;
using SFDocGen.Services;
using System.IO.Compression;
using System.Net.Mime;

namespace SFDocGen.Controllers;

[ApiController]
[Route("docs")]
public class DocsController : Controller
{
    [HttpGet("full")]
    [Produces(MediaTypeNames.Application.Zip)]
    [EndpointSummary("Returns the full, uncompressed Lua documentation.")]
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
}
