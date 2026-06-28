using Model;
using System.Text;

namespace SFDocGen.Services;

public class LuaGenerator(ILogger<LuaGenerator> logger)
{
    public const string MINDOC_PATH = "Storage/starfall-docs.min.lua";
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

        using FileStream minStream = File.OpenWrite(MINDOC_PATH);
        using TextWriter minWriter = new StreamWriter(minStream);

        minWriter.WriteLine("---@meta Starfall");
        AddDiagnostic(minWriter, "keyword", "assign-type-mismatch");

        string hookPath = Path.Combine(HOOKS_PATH, "Hooks.lua");
        using (StreamWriter hookWriter = new(File.OpenWrite(hookPath)))
        {
            hookWriter.WriteLine("---@meta Hooks");
            AddDiagnostic(hookWriter, "keyword", "assign-type-mismatch");

            MultiWrite("\n", hookWriter, minWriter);

            foreach (var hook in documentation.Hooks.Values.OrderBy(h => h.Name))
            {
                MultiWrite(hook.ToLuaDoc(), hookWriter, minWriter);
                MultiWrite("\n", hookWriter, minWriter);
            }

            MultiWrite("---@overload fun(hookName: string, name: string, callback?: function)", hookWriter, minWriter);
            MultiWriteLine(null, hookWriter, minWriter);
            MultiWrite("hook = nil", hookWriter, minWriter);
            minWriter.WriteLine();
        }


        foreach (var library in documentation.Libraries.Values.OrderBy(l => l.Name))
        {
            string path = Path.Combine(LIBRARIES_PATH, library.Name + ".lua");
            using StreamWriter libWriter = new(File.OpenWrite(path));

            libWriter.WriteLine($"---@meta {library.Name}");
            AddDiagnostic(libWriter, "keyword");

            MultiWriteLine(null, libWriter, minWriter);
            MultiWrite(library.ToLuaDoc(), libWriter, minWriter);
            minWriter.WriteLine();
        }

        foreach (var luaClass in documentation.Classes.Values.OrderBy(c => c.Name))
        {
            string path = Path.Combine(CLASSES_PATH, luaClass.Name + ".lua");
            using StreamWriter classWriter = new(File.OpenWrite(path));

            classWriter.WriteLine($"---@meta {luaClass.Name}");
            AddDiagnostic(classWriter, "keyword");

            MultiWriteLine(null, classWriter, minWriter);
            MultiWrite(luaClass.ToLuaDoc(), classWriter, minWriter);
            minWriter.WriteLine();
        }

        foreach (var table in documentation.Tables.Values.OrderBy(t => t.Name))
        {
            string path = Path.Combine(TABLES_PATH, table.Name + ".lua");
            using StreamWriter tableWriter = new(File.OpenWrite(path));

            tableWriter.WriteLine($"---@meta {table.Name}");
            AddDiagnostic(tableWriter, "keyword");

            MultiWriteLine(null, tableWriter, minWriter);
            MultiWrite(table.ToLuaDoc(), tableWriter, minWriter);
            minWriter.WriteLine();
        }
    }

    protected void AddDiagnostic(TextWriter writer, params string[] names)
    {
        writer.WriteLine($"---@diagnostic disable: {string.Join(", ", names)}");
    }

    protected void MultiWrite(string? value, params TextWriter[] writers)
    {
        foreach (var writer in writers)
        {
            writer.Write(value);
        }
    }

    protected void MultiWriteLine(string? value, params TextWriter[] writers)
    {
        foreach (var writer in writers)
        {
            writer.WriteLine(value);
        }
    }
}