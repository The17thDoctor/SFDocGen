using Model;

namespace SFDocGen.Core;

/// <summary>
/// Manages the files inside the Storage folder & the up to date Starfall documentation object.
/// </summary>
public class StorageManager
{
    public SFDocRoot Documentation { get; set; } = new();

    public StorageFolders Folders { get; }
    public StorageFiles Files { get; }

    public StorageManager(IConfiguration configuration)
    {
        string root = configuration.GetValue<string>("StorageFolder") ?? "Storage";

        Folders = new StorageFolders(root);
        Files = new StorageFiles(root);

        Directory.CreateDirectory(Folders.Root);
    }

    public class StorageFolders(string root)
    {
        public readonly string Root = root;
        public LuaDocFolders LuaDoc { get; } = new(root);

        public class LuaDocFolders(string root)
        {
            public readonly string Root = Path.Combine(root, "Lua");

            public readonly string Classes = Path.Combine(root, "classes");
            public readonly string Directives = Path.Combine(root);
            public readonly string Hooks = Path.Combine(root);
            public readonly string Libraries = Path.Combine(root, "libraries");
            public readonly string Tables = Path.Combine(root, "tables");
        }
    }

    public class StorageFiles(string root)
    {
        public readonly string OriginalDoc = Path.Combine(root, "docs-original.json");
        public readonly string ImprovedDoc = Path.Combine(root, "docs-improved.json");
        public readonly string ImprovedDocSchema = Path.Combine(root, "docs-improved.schema.json");
        public readonly string CorrectionsFile = Path.Combine(root, "docs-corrections.json");
        public readonly string MinifiedLuaDocs = Path.Combine(root, "starfall.min.lua");
    }
}