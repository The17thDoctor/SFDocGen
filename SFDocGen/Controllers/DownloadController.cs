using Microsoft.AspNetCore.Mvc;
using SFDocGen.Core;
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
        if (!Directory.Exists(StorageManager.Folders.LuaDoc.Root))
        {
            return NotFound();
        }

        MemoryStream stream = new();
        ZipFile.CreateFromDirectory(StorageManager.Folders.LuaDoc.Root, stream);

        stream.Position = 0;

        return File(stream, MediaTypeNames.Application.Zip);
    }

    [HttpGet("lua/min")]
    [Produces("text/x-lua")]
    [EndpointSummary("Returns the minified Starfall documentation.")]
    public ActionResult MinifiedDocumentation()
    {
        if (!System.IO.File.Exists(StorageManager.Files.MinifiedLuaDocs))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(StorageManager.Files.MinifiedLuaDocs);
        return File(file, "text/x-lua");
    }

    [HttpGet("json/full")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the documentation as a JSON file.")]
    public ActionResult JsonDocumentation()
    {
        if (!System.IO.File.Exists(StorageManager.Files.ImprovedDoc))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(StorageManager.Files.ImprovedDoc);
        return File(file, MediaTypeNames.Application.Json);
    }

    [HttpGet("json/schema")]
    [Produces(MediaTypeNames.Application.Json)]
    [EndpointSummary("Returns the documentation's JSON schema.")]
    public ActionResult JsonSchema()
    {
        if (!System.IO.File.Exists(StorageManager.Files.ImprovedDocSchema))
        {
            return NotFound();
        }

        FileStream file = System.IO.File.OpenRead(StorageManager.Files.ImprovedDocSchema);
        return File(file, MediaTypeNames.Application.Json);
    }
}
