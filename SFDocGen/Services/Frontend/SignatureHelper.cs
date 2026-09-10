using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Services.Frontend;

public static class SignatureHelper
{
    public static string Create(this SFDocValue value)
    {
        return value switch
        {
            SFLibraryField field => Create(field),
            SFLibraryFunction function => Create(function),
            SFLibraryTable table => Create(table),
            SFClassField field => Create(field),
            SFClassMethod method => Create(method),
            SFClassOperator @operator => Create(@operator),
            SFTableField field => Create(field),
            _ => value.DocName ?? value.Name ?? string.Empty
        };
    }

    // Library
    public static string Create(this SFLibraryField field)
    {
        if (field.Parent.Name == "builtin") return $"{field.DocName ?? field.Name}";
        else return $"{field.Parent.DocName ?? field.Parent.Name}.{field.DocName ?? field.Name}";
    }

    public static string Create(this SFLibraryFunction function, bool includeParameters = false, bool includeFunctionKeyword = false)
    {
        string parameterList = includeParameters ? string.Join(", ", function.Parameters.Select(p => p.Name)) : string.Empty;
        string functionKeyword = includeFunctionKeyword ? "function " : string.Empty;
        string prefix = (function.Parent.Name == "builtin") ? string.Empty : $"{function.Parent.DocName ?? function.Parent.Name}.";

        return $"{functionKeyword}{prefix}{function.DocName ?? function.Name}({parameterList})";
    }

    public static string Create(this SFLibraryTable table, bool includeAssignment = false)
    {
        string prefix = (table.Parent.Name == "builtin") ? string.Empty : $"{table.Parent.DocName ?? table.Parent.Name}.";
        string tableAssignment = includeAssignment ? " = {}" : string.Empty;

        return $"{prefix}{table.DocName ?? table.Name}{tableAssignment}";
    }


    // Class
    public static string Create(this SFClassField field)
    {
        return $"{field.Parent.DocName ?? field.Parent.Name}.{field.DocName ?? field.Name}";
    }

    public static string Create(this SFClassMethod method, bool includeParameters = false, bool includeFunctionKeyword = false)
    {
        string parameterList = includeParameters ? string.Join(", ", method.Parameters.Select(p => p.Name)) : string.Empty;
        string functionKeyword = includeFunctionKeyword ? "function " : string.Empty;
        return $"{functionKeyword}{method.Parent.DocName ?? method.Parent.Name}:{method.DocName ?? method.Name}({parameterList})";
    }

    private static readonly Dictionary<string, string> _operatorFormats = new()
    {
        { "mul", "{0} * {1}" },
        { "div", "{0} / {1}" },
        { "eq",  "{0} == {1}" },
        { "add", "{0} + {1}" },
        { "sub", "{0} - {1}" },
        { "pow", "{0} ^ {1}" },
        { "unm", "-{0}" }
    };

    public static string Create(this SFClassOperator @operator)
    {
        string opId = @operator.Name!.Split("_")[0];
        string format = _operatorFormats.GetValueOrDefault(opId, @operator.Name!);
        return string.Format(format, @operator.LeftOperand, @operator.RightOperand);
    }


    // Table
    public static string Create(this SFTable table)
    {
        string result = $"{table.DocName ?? table.Name}\n{{\n\t";

        result += string.Join(",\n\t", table.Fields.Values.Select(field => $"{field.DocName ?? field.Name}: {field.Type}"));
        result += "\n}";

        return result;
    }

    public static string Create(this SFTableField field)
    {
        return $"{field.Parent.DocName ?? field.Parent.Name}.{field.DocName ?? field.Name}";
    }


    // Hook
    public static string Create(this SFHook hook)
    {
        string parameterList = string.Join(", ", hook.Parameters.Select(p => p.Name));
        return $"hook(\"{hook.Name}\", \"hookname\", function({parameterList}) end)";
    }


    // Directive
    public static string Create(this SFDirective directive)
    {
        string parameterList = string.Join(" ", directive.Parameters.Select(p => $"<{p.Name}>"));
        return $"--@{directive.Name} {parameterList}";
    }
}