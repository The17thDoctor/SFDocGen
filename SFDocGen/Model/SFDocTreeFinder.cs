using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Model;

public class SFDocTreeFinder
{
    public static List<SFDocValue> Find(SFDocRoot root, Predicate<SFDocValue> filter)
    {
        SFDocTreeVisitor visitor = new(filter);
        root.Accept(visitor);

        return visitor.Result;
    }
}

file class SFDocTreeVisitor(Predicate<SFDocValue> filter) : IDocumentationVisitor
{
    public List<SFDocValue> Result { get; } = [];

    public void VisitAlias(SFTypeAlias alias) => VisitValue(alias);
    public void VisitClass(SFClass @class) => VisitValue(@class);
    public void VisitClassField(SFClassField field) => VisitValue(field);
    public void VisitClassMethod(SFClassMethod method) => VisitValue(method);
    public void VisitClassOperator(SFClassOperator @operator) => VisitValue(@operator);
    public void VisitDirective(SFDirective directive) => VisitValue(directive);
    public void VisitFunctionOverload(SFFunctionOverload overload) => VisitValue(overload);
    public void VisitHook(SFHook hook) => VisitValue(hook);
    public void VisitLibrary(SFLibrary library) => VisitValue(library);
    public void VisitLibraryField(SFLibraryField field) => VisitValue(field);
    public void VisitLibraryFunction(SFLibraryFunction function) => VisitValue(function);
    public void VisitLibraryTable(SFLibraryTable table) => VisitValue(table);
    public void VisitParameter(SFParameter parameter) => VisitValue(parameter);
    public void VisitReturnValue(SFReturnValue value) => VisitValue(value);
    public void VisitTable(SFTable table) => VisitValue(table);
    public void VisitTableField(SFTableField field) => VisitValue(field);

    private void VisitValue(SFDocValue value)
    {
        if (filter(value)) Result.Add(value);
    }
}