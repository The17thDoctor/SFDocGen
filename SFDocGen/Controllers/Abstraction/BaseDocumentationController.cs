using Microsoft.AspNetCore.Mvc;
using SFDocGen.Core;
using SFDocGen.Model.Core;
using SFDocGen.Model.Starfall;
using System.Text.Json;

namespace SFDocGen.Controllers.Abstraction;

public abstract class BaseDocumentationController : Controller
{
    protected StorageManager Storage { get; init; }
    protected SFDocRoot Documentation => Storage.Documentation;
    protected JsonSerializerOptions SerializerOptions { get; init; } = new();

    public BaseDocumentationController(StorageManager storage)
    {
        Storage = storage;
        SerializerOptions.Converters.Add(new RealmConverter());
    }
}
