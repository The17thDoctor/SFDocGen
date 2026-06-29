using SFDocGen.Core;
using LuaFolders = SFDocGen.Core.StorageManager.Folders.LuaDoc;

namespace SFDocGen.Services;

/// <summary>
/// Generates the LuaLS formatted documentation from the current data.
/// </summary>
public class LuaGenerator(ILogger<LuaGenerator> logger, StorageManager storage)
{
    public void GenerateLuaDoc()
    {
        logger.LogInformation("Generating lua documentation...");

        if (Directory.Exists(LuaFolders.Root))
        {
            Directory.Delete(LuaFolders.Root, recursive: true);
        }

        Directory.CreateDirectory(LuaFolders.Root);
        Directory.CreateDirectory(LuaFolders.Classes);
        Directory.CreateDirectory(LuaFolders.Directives);
        Directory.CreateDirectory(LuaFolders.Hooks);
        Directory.CreateDirectory(LuaFolders.Libraries);
        Directory.CreateDirectory(LuaFolders.Tables);

        var documentation = storage.Documentation;
        using FileStream minStream = File.OpenWrite(StorageManager.Files.MinifiedLuaDocs);
        using TextWriter minWriter = new StreamWriter(minStream);

        minWriter.WriteLine("---@meta Starfall");
        AddDiagnostic(minWriter, "keyword", "assign-type-mismatch");

        string hookPath = Path.Combine(LuaFolders.Hooks, "Hooks.lua");
        using (StreamWriter hookWriter = new(File.OpenWrite(hookPath)))
        {
            WriteHook(documentation, minWriter, hookWriter);
        }

        foreach (var library in documentation.Libraries.Values.OrderBy(l => l.Name))
        {
            WriteLibrary(minWriter, library);
        }

        foreach (var luaClass in documentation.Classes.Values.OrderBy(c => c.Name))
        {
            WriteClass(minWriter, luaClass);
        }

        foreach (var table in documentation.Tables.Values.OrderBy(t => t.Name))
        {
            WriteTable(minWriter, table);
        }

        logger.LogInformation("Lua documentation generated.");
    }

    private void WriteTable(TextWriter minWriter, Model.SFTable table)
    {
        string path = Path.Combine(LuaFolders.Tables, table.Name + ".lua");
        using StreamWriter tableWriter = new(File.OpenWrite(path));

        tableWriter.WriteLine($"---@meta {table.Name}");
        AddDiagnostic(tableWriter, "keyword");

        MultiWriteLine(null, tableWriter, minWriter);
        MultiWrite(table.ToLuaDoc(), tableWriter, minWriter);
        minWriter.WriteLine();
    }

    private void WriteClass(TextWriter minWriter, Model.SFClass luaClass)
    {
        string path = Path.Combine(LuaFolders.Classes, luaClass.Name + ".lua");
        using StreamWriter classWriter = new(File.OpenWrite(path));

        classWriter.WriteLine($"---@meta {luaClass.Name}");
        AddDiagnostic(classWriter, "keyword");

        MultiWriteLine(null, classWriter, minWriter);
        MultiWrite(luaClass.ToLuaDoc(), classWriter, minWriter);
        minWriter.WriteLine();
    }

    private void WriteLibrary(TextWriter minWriter, Model.SFLibrary library)
    {
        string path = Path.Combine(LuaFolders.Libraries, library.Name + ".lua");
        using StreamWriter libWriter = new(File.OpenWrite(path));

        libWriter.WriteLine($"---@meta {library.Name}");
        AddDiagnostic(libWriter, "keyword");

        MultiWriteLine(null, libWriter, minWriter);
        MultiWrite(library.ToLuaDoc(), libWriter, minWriter);
        minWriter.WriteLine();
    }

    private void WriteHook(global::Model.SFDocRoot documentation, TextWriter minWriter, StreamWriter hookWriter)
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