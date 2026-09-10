using Microsoft.AspNetCore.Mvc.Abstractions;
using SFDocGen.Blazor.Pages.APIViews.Library;
using SFDocGen.Core;
using SFDocGen.Model;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;
using System.Text;
using System.Xml.Linq;
using static MudBlazor.CategoryTypes;

namespace SFDocGen.Services.Backend;

/// <summary>
/// Manages the creation and writing of LuaLS-formatted documentation files.
/// </summary>
public class LuaGenerator(ILogger<LuaGenerator> logger, StorageManager storage, ConfigManager configs)
{
    private readonly StorageManager.StorageFolders.LuaDocFolders _luaFolders = storage.Folders.LuaDoc;

    public void GenerateLuaDoc()
    {
        logger.LogInformation("Generating lua documentation...");

        if (Directory.Exists(_luaFolders.Root))
        {
            Directory.Delete(_luaFolders.Root, recursive: true);
        }

        Directory.CreateDirectory(_luaFolders.Root);
        Directory.CreateDirectory(_luaFolders.Classes);
        Directory.CreateDirectory(_luaFolders.Dependencies);
        Directory.CreateDirectory(_luaFolders.Directives);
        Directory.CreateDirectory(_luaFolders.Hooks);
        Directory.CreateDirectory(_luaFolders.Libraries);
        Directory.CreateDirectory(_luaFolders.Tables);

        SFDocRoot documentation = storage.Documentation;

        using TextWriter minWriter = CreateFile(storage.Files.MinifiedLuaDocs, "Starfall", ["keyword", "assign-type-mismatch"]);
        LuaWriterVisitor minVisitor = new(minWriter);

        // Aliases
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Root, "aliases.lua");
            using TextWriter aliasWriter = CreateFile(path, "Starfall.Aliases", []);
            LuaWriterVisitor aliasVisitor = new(aliasWriter);

            foreach (var alias in documentation.Aliases.Values)
            {
                alias.Accept(aliasVisitor);
                alias.Accept(minVisitor);
            }
        }

        minWriter.WriteLine();

        // Hooks
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Root, "hooks.lua");
            using TextWriter hookWriter = CreateFile(path, "Starfall.Hooks", []);
            LuaWriterVisitor hookVisitor = new(hookWriter);

            hookWriter.WriteLine("---@overload fun(hookName: string, name: string, callback?: function)");
            minWriter.WriteLine("---@overload fun(hookName: string, name: string, callback?: function)");

            foreach (var hook in documentation.Hooks.Values)
            {
                hook.Accept(hookVisitor);
                hook.Accept(minVisitor);
            }

            hookWriter.WriteLine("hook = nil");
            minWriter.WriteLine("hook = nil");
            minWriter.WriteLine();
        }

        // Directives
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Root, "directives.lua");
            using TextWriter directiveWriter = CreateFile(path, "Starfall.Directives", []);
            LuaWriterVisitor directiveVisitor = new(directiveWriter);

            foreach (var directive in documentation.Directives.Values)
            {
                directive.Accept(directiveVisitor);
                directive.Accept(minVisitor);

                minWriter.WriteLine();
            }

            minWriter.WriteLine();
        }

        // Tables
        foreach (var table in documentation.Tables.Values)
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Tables, $"{table.Name}.lua");
            using TextWriter tableWriter = CreateFile(path, $"Starfall.Structure.{table.Name}", []);
            LuaWriterVisitor tableVisitor = new(tableWriter);

            table.Accept(tableVisitor);
            table.Accept(minVisitor);

            minWriter.WriteLine();
        }

        // Classes
        foreach (var @class in documentation.Classes.Values)
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Classes, $"{@class.Name}.lua");
            using TextWriter classWriter = CreateFile(path, $"Starfall.Class.{@class.Name}", ["keyword"]);
            LuaWriterVisitor classVisitor = new(classWriter);

            @class.Accept(classVisitor);
            @class.Accept(minVisitor);

            minWriter.WriteLine();
        }

        // Libraries
        foreach (var library in documentation.Libraries.Values)
        {
            string path = Path.Combine(storage.Folders.LuaDoc.Libraries, $"{library.Name}.lua");
            using TextWriter libraryWriter = CreateFile(path, $"Starfall.Library.{library.Name}", ["keyword"]);
            LuaWriterVisitor libraryVisitor = new(libraryWriter);

            library.Accept(libraryVisitor);
            library.Accept(minVisitor);

            minWriter.WriteLine();
        }

        // Dependencies
        foreach (Dependency dep in configs.GetDependencies())
        {
            string sourcePath = Path.Combine(storage.Folders.DependenciesFolder, $"{dep.Name}.lua");
            string destinationPath = Path.Combine(storage.Folders.LuaDoc.Dependencies, $"{dep.Name}.lua");

            logger.LogInformation("Copying external dependency: {name}", dep.Name);
            File.Copy(sourcePath, destinationPath);

            minWriter.WriteLine();
            foreach (var line in File.ReadAllLines(destinationPath))
            {
                if (line.StartsWith("---@meta")) continue;
                minWriter.WriteLine(line);
            }
            minWriter.WriteLine();
        }

        logger.LogInformation("Lua documentation generated.");
    }

    protected TextWriter CreateFile(string path, string meta, string[] diagnostics)
    {
        TextWriter writer = new StreamWriter(path, new FileStreamOptions()
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write
        });

        writer.WriteLine($"---@meta {meta}");
        
        if (diagnostics.Length != 0)
        {
            writer.WriteLine($"---@diagnostic disable: {string.Join(", ", diagnostics)}");
        }

        writer.WriteLine();

        return writer;
    }
}


/// <summary>
/// Implementation of <see cref="IDocumentationVisitor"/> which writes LuaLS-formatted documentation.
/// </summary>
/// <param name="writer">The writer used to write the Lua documentation.</param>
public class LuaWriterVisitor(TextWriter writer) : IDocumentationVisitor
{
    /// <summary>
    /// A list of operators that are purposely removed from the documentation generation
    /// as they are not supported by the LuaLS notation.
    /// </summary>
    private static readonly List<string> _bannedOperators = ["eq", "lt", "gt"];

    public void VisitAlias(SFTypeAlias alias)
    {
        StringBuilder builder = new();

        builder.Append(FormatDescription(alias.Description));
        builder.Append($"---@alias {alias.Name} ");
        builder.AppendJoin('|', alias.Types);

        writer.WriteLine(builder.ToString());
    }

    public void VisitDirective(SFDirective directive)
    {
        StringBuilder builder = new();

        builder.Append(FormatDescription(directive.Description));
        builder.Append(FormatUsage(directive.Usage));

        builder.Append($"---@directive {directive.Name} ");
        builder.AppendJoin(" ", directive.Parameters.Select(p => $"<{p.Name}>"));

        writer.WriteLine(builder.ToString().TrimEnd());
    }

    public void VisitHook(SFHook hook)
    {
        StringBuilder builder = new();
        builder.Append($"---@overload fun(hookName: \"{hook.Name}\", name: string, callback?: fun(");
        builder.AppendJoin(", ", hook.Parameters.Select(p => $"{p.Name}: {ConcatTypes(p)}"));
        builder.Append(')');

        if (hook.ReturnValues.Count > 0)
        {
            builder.Append(": ");
            builder.AppendJoin(", ", hook.ReturnValues.Select(ConcatTypes));
        }

        builder.Append(')');

        if (hook.Description != null)
        {
            builder.Append(' ');
            builder.Append(hook.Description.Replace("\n", "<br>"));
        }

        writer.WriteLine(builder.ToString());
    }


    // Class
    public void VisitClass(SFClass @class)
    {
        StringBuilder builder = new();

        builder.Append(FormatDescription(@class.Description));
        builder.Append(FormatUsage(@class.Usage));

        builder.Append($"---@class {@class.DocName ?? @class.Name}");
        if (@class.SuperType != null) builder.Append(" : " + @class.SuperType);
        builder.AppendLine();

        foreach (var field in @class.Fields.Values)
        {
            builder.Append($"---@field {field.Name} {field.Type ?? "unknown"} ");
            builder.Append((field.Description ?? string.Empty).Replace("\n", "<br/>"));
            builder.AppendLine();
        }

        builder.AppendLine($"{@class.DocName ?? @class.Name} = {{}}");

        writer.WriteLine(builder.ToString());
    }

    // Handled by the VisitClass method because of text order issues.
    public void VisitClassField(SFClassField field) { }

    public void VisitClassMethod(SFClassMethod method)
    {
        writer.WriteLine(FormatFunction(method));
    }

    public void VisitClassOperator(SFClassOperator @operator)
    {
        string opName = @operator.Name?.Split('_')[0] ?? string.Empty;
        if (_bannedOperators.Contains(opName)) return;

        StringBuilder builder = new();

        builder.Append($"---@operator {opName}");

        string returnType = ConcatTypes(@operator.ReturnValues.First());
        if (@operator.RightOperand is null or "nil")
        {
            builder.Append($": {returnType}");
        }
        else
        {
            builder.Append($"({@operator.RightOperand}): {returnType}");
        }

        writer.WriteLine(builder.ToString());
    }


    // Library
    public void VisitLibrary(SFLibrary library)
    {
        StringBuilder builder = new();

        builder.Append(FormatDescription(library.Description));
        builder.Append(FormatUsage(library.Usage));
        builder.AppendLine($"{library.DocName ?? library.Name} = {{}}");

        writer.WriteLine(builder.ToString());
    }

    public void VisitLibraryField(SFLibraryField field)
    {
        string prefix = $"{field.Parent.DocName ?? field.Parent.Name}.";
        if (field.Parent.DocName == "_G") prefix = string.Empty;

        StringBuilder builder = new();

        builder.Append(FormatDescription(field.Description));
        builder.Append(FormatDescription(field.Usage));
        builder.AppendLine($"{prefix}{field.DocName ?? field.Name} = {field.Value}");

        writer.WriteLine(builder.ToString());
    }

    public void VisitLibraryFunction(SFLibraryFunction function)
    {
        writer.WriteLine(FormatFunction(function));
    }

    public void VisitLibraryTable(SFLibraryTable table)
    {
        string prefix = $"{table.Parent.DocName ?? table.Parent.Name}.";
        if (table.Parent.DocName == "_G") prefix = string.Empty;

        StringBuilder builder = new();

        builder.Append(FormatDescription(table.Description));
        builder.Append(FormatUsage(table.Usage));
        builder.AppendLine($"{prefix}{table.DocName ?? table.Name} = {{}}");

        writer.WriteLine(builder.ToString());
    }


    // Table
    public void VisitTable(SFTable table)
    {
        StringBuilder builder = new();

        builder.Append(FormatDescription(table.Description));
        builder.Append(FormatUsage(table.Usage));
        builder.AppendLine($"---@class {table.Name}");

        builder.Append($"{table.Name} = {{}}\n");
        writer.WriteLine(builder.ToString());
    }

    public void VisitTableField(SFTableField field)
    {
        StringBuilder builder = new();
        builder.Append(FormatDescription(field.Description));
        builder.AppendLine($"---@type {field.Type ?? "unknown"}");
        builder.AppendLine($"{field.Parent.Name}.{field.Name} = {field.DefaultValue}");

        writer.WriteLine(builder.ToString());
    }


    // Base Elements
    public void VisitParameter(SFParameter parameter) { }
    public void VisitReturnValue(SFReturnValue value) { }
    public void VisitFunctionOverload(SFFunctionOverload overload) { }


    // Common Methods
    protected string FormatDescription(string? description)
    {
        if (description is null) return string.Empty;
        return "---" + description.Replace("\n", "\n---<br/>") + "\n";
    }

    protected string FormatUsage(string? usage)
    {
        if (usage is null) return string.Empty;

        string result = "---<br/><br/>\n";
        result += "---Usage:\n";
        result += "---```lua\n";
        result += $"---{usage.Replace("\n", "\n---")}";
        result += "\n---```\n";

        return result;
    }

    protected string FormatFunction<T>(SFFunction<T> function) where T : SFDocValue
    {
        string separator = (typeof(T) == typeof(SFClass)) ? ":" : ".";
        string prefix = function.Parent.Name + separator;

        // Special case for builtin library functions.
        if (function.Parent.DocName == "_G") prefix = string.Empty;

        StringBuilder builder = new();

        builder.Append(FormatDescription(function.Description));
        builder.Append(FormatUsage(function.Usage));

        if (function.Deprecated != null)
        {
            builder.AppendLine($"---@deprecated {function.Deprecated.Replace("\n", "<br>\n---")}");
        }

        if (function.GenericTypes.Count > 0)
        {
            builder.Append("---@generic ");
            builder.AppendJoin(", ", function.GenericTypes);
            builder.AppendLine();
        }

        foreach (var parameter in function.Parameters)
        {
            builder.AppendLine(FormatParameter(parameter));
        }

        foreach (var returnValue in function.ReturnValues)
        {
            builder.AppendLine(FormatReturnValue(returnValue));
        }

        foreach (var overload in function.Overloads)
        {
            builder.AppendLine(FormatFunctionOverload(overload));
        }

        builder.Append($"function {prefix}{function.Name}(");
        builder.AppendJoin(", ", function.Parameters.Select(p => p.Name));
        builder.AppendLine(") end");

        return builder.ToString();
    }

    protected string FormatReturnValue(SFReturnValue returnValue)
    {
        StringBuilder sb = new();
        sb.Append($"---@return {ConcatTypes(returnValue)}");

        if (returnValue.Name != null) { sb.Append($" {returnValue.Name}"); }
        if (returnValue.Description != null) { sb.Append(" '" + returnValue.Description.Replace("\n", "<br>").Replace("'", "\\'") + "'"); }

        return sb.ToString();
    }

    protected string FormatParameter(SFParameter parameter)
    {
        StringBuilder sb = new();
        sb.Append($"---@param {parameter.Name} {parameter.ConcatTypes()} ");

        if (parameter.Description != null) sb.Append(parameter.Description.Replace("\n", "<br>\n---"));

        return sb.ToString();
    }

    protected string FormatFunctionOverload(SFFunctionOverload overload)
    {
        StringBuilder builder = new();

        builder.Append("---@overload fun(");
        builder.AppendJoin(", ", overload.Parameters.Select(p => $"{p.Name}: {ConcatTypes(p)}"));
        builder.Append(')');

        if (overload.ReturnValues.Count > 0)
        {
            builder.Append(": ");
            builder.AppendJoin(", ", overload.ReturnValues.Select(ConcatTypes));
        }

        return builder.ToString();
    }

    protected string ConcatTypes(SFParameter parameter)
    {
        if (parameter.Types.Count == 0) return "unknown";
        return string.Join("|", parameter.Types);
    }

    protected string ConcatTypes(SFReturnValue returnValue)
    {
        if (returnValue.Types.Count == 0) return "unknown";
        return string.Join("|", returnValue.Types);
    }
}