using SFDocGen.Model.Starfall;

namespace SFDocGen.Services.Frontend;

/// <summary>
/// Maps the various SF Document Values to URIs for the API.
/// </summary>
public static class SFDocUriMapper
{
    public const string ApiPrefix = "/ref";

    // Hook

    public static string Map(SFHook hook)
    {
        return $"{ApiPrefix}/hook/{hook.Name}";
    }

    // Class

    public static string Map(SFClass @class)
    {
        return $"{ApiPrefix}/class/{@class.Name}";
    }

    public static string Map(SFClassField field)
    {
        return $"{Map(field.Parent)}/field/{field.Name}";
    }

    public static string Map(SFClassMethod method)
    {
        return $"{Map(method.Parent)}/method/{method.Name}";
    }

    public static string Map(SFClassOperator @operator)
    {
        return $"{Map(@operator.Parent)}/operator/{@operator.Name}";
    }

    // Library

    public static string Map(SFLibrary library)
    {
        return $"{ApiPrefix}/library/{library.Name}";
    }

    public static string Map(SFLibraryField field)
    {
        return $"{Map(field.Parent)}/field/{field.Name}";
    }

    public static string Map(SFLibraryFunction function)
    {
        return $"{Map(function.Parent)}/function/{function.Name}";
    }

    public static string Map(SFLibraryTable table)
    {
        return $"{Map(table.Parent)}/table/{table.Name}";
    }

    // Table

    public static string Map(SFTable table)
    {
        return $"{ApiPrefix}/table/{table.Name}";
    }

    public static string Map(SFTableField field)
    {
        return $"{Map(field.Parent)}/field/{field.Name}";
    }

    // Directive

    public static string Map(SFDirective directive)
    {
        return $"{ApiPrefix}/directive/{directive.Name}";
    }
}
