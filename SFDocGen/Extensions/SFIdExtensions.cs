using SFDocGen.Model.Starfall;

namespace SFDocGen.Extensions;

/// <summary>
/// Provides methods for generating a unique ID for a given Starfall documentation value.
/// </summary>
public static class SFIdExtensions
{
    public static string Id(this SFHook hook)
    {
        return $"hook/{hook.Name}";
    }

    public static string Id(this SFDirective directive)
    {
        return $"directive/{directive.Name}";
    }

    public static string Id(this SFTypeAlias alias)
    {
        return $"alias/{alias.Name}";
    }


    // Table
    public static string Id(this SFTable table)
    {
        return $"table/{table.Name}";
    }

    public static string Id(this SFTableField field)
    {
        return $"{field.Parent.Id()}/field/{field.Name}";
    }


    // Class
    public static string Id(this SFClass @class)
    {
        return $"class/{@class.Name}";
    }

    public static string Id(this SFClassField field)
    {
        return $"{field.Parent.Id()}/field/{field.Name}";
    }

    public static string Id(this SFClassMethod method)
    {
        return $"{method.Parent.Id()}/method/{method.Name}";
    }

    public static string Id(this SFClassOperator @operator)
    {
        return $"{@operator.Parent.Id()}/operator/{@operator.Name}";
    }


    // Library
    public static string Id(this SFLibrary library)
    {
        return $"library/{library.Name}";
    }

    public static string Id(this SFLibraryField field)
    {
        return $"{field.Parent.Id()}/field/{field.Name}";
    }

    public static string Id(this SFLibraryFunction function)
    {
        return $"{function.Parent.Id()}/function/{function.Name}";
    }

    public static string Id(this SFLibraryTable table)
    {
        return $"{table.Parent.Id()}/table/{table.Name}";
    }
}