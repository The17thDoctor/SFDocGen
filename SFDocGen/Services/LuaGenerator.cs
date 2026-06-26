using Model;

namespace SFDocGen.Services;

public class LuaGenerator(ILogger<LuaGenerator> logger)
{
    public const string LUADOC_PATH = "Storage/LuaDoc";
    public const string HOOKS_PATH = "Storage/LuaDoc";
    public const string LIBRARIES_PATH = "Storage/LuaDoc/Libraries";
    public const string CLASSES_PATH = "Storage/LuaDoc/Classes";
    public const string TABLES_PATH = "Storage/LuaDoc/Tables";

    public void GenerateLuaDoc(SFDocRoot documentation)
    {
        logger.LogInformation("Generating lua documentation...");
        if (documentation == null)
        {
            logger.LogWarning("Documentation model not found!");
            return;
        }

        if (Directory.Exists(LUADOC_PATH)) Directory.Delete(LUADOC_PATH, recursive: true);

        Directory.CreateDirectory(LUADOC_PATH);
        Directory.CreateDirectory(HOOKS_PATH);
        Directory.CreateDirectory(LIBRARIES_PATH);
        Directory.CreateDirectory(CLASSES_PATH);
        Directory.CreateDirectory(TABLES_PATH);

        string hookPath = Path.Combine(HOOKS_PATH, "Hooks.lua");
        using (FileStream hookStream = File.OpenWrite(hookPath))
        {
            using StreamWriter hookWriter = new(hookStream);
            hookWriter.WriteLine("---@meta Hooks");
            AddDiagnostic(hookWriter, "keyword", "assign-type-mismatch");
            hookWriter.WriteLine();

            foreach (var hook in documentation.Hooks.Values.OrderBy(h => h.Name))
            {
                hookWriter.WriteLine(hook.ToLuaDoc());
            }

            hookWriter.WriteLine("---@overload fun(hookName: string, name: string, callback?: function)");
            hookWriter.Write("hook = nil");
        }

        foreach (var library in documentation.Libraries.Values.OrderBy(l => l.Name))
        {
            string path = Path.Combine(LIBRARIES_PATH, library.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {library.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(library.ToLuaDoc());
        }

        foreach (var luaClass in documentation.Classes.Values.OrderBy(c => c.Name))
        {
            string path = Path.Combine(CLASSES_PATH, luaClass.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {luaClass.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(luaClass.ToLuaDoc());
        }

        foreach (var table in documentation.Tables.Values.OrderBy(t => t.Name))
        {
            string path = Path.Combine(TABLES_PATH, table.Name + ".lua");
            using FileStream stream = File.OpenWrite(path);
            using StreamWriter writer = new(stream);

            writer.WriteLine($"---@meta {table.Name}");
            AddDiagnostic(writer, "keyword");
            writer.WriteLine();

            writer.Write(table.ToLuaDoc());
        }
    }

    protected void AddDiagnostic(TextWriter writer, params string[] names)
    {
        writer.WriteLine($"---@diagnostic disable: {string.Join(", ", names)}");
    }
}
