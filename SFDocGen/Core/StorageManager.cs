using Model;

namespace SFDocGen.Core;

/// <summary>
/// Manages the files inside the Storage folder & the up to date Starfall documentation object.
/// </summary>
public class StorageManager
{
    public SFDocRoot Documentation { get; set; } = new();

    public static class Folders
    {
        public static readonly string Root = "Storage";

        public static class LuaDoc
        {
            public static readonly string Root = Path.Combine(Folders.Root, "Lua");

            public static readonly string Classes = Path.Combine(Root, "classes");
            public static readonly string Directives = Path.Combine(Root);
            public static readonly string Hooks = Path.Combine(Root);
            public static readonly string Libraries = Path.Combine(Root, "libraries");
            public static readonly string Tables = Path.Combine(Root, "tables");
        }
    }

    public static class Files
    {
        public static readonly string OriginalDoc = Path.Combine(Folders.Root, "docs-original.json");
        public static readonly string ImprovedDoc = Path.Combine(Folders.Root, "docs-improved.json");
        public static readonly string ImprovedDocSchema = Path.Combine(Folders.Root, "docs-improved.schema.json");
        public static readonly string MinifiedLuaDocs = Path.Combine(Folders.Root, "starfall.min.lua");
    }
}