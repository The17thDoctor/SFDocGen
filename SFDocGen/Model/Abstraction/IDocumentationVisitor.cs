using SFDocGen.Model.Starfall;

namespace SFDocGen.Model.Abstraction;

public interface IDocumentationVisitor
{
    void VisitHook(SFHook hook);
    void VisitDirective(SFDirective directive);
    void VisitAlias(SFTypeAlias alias);

    void VisitTable(SFTable table);
    void VisitTableField(SFTableField field);

    void VisitLibrary(SFLibrary library);
    void VisitLibraryField(SFLibraryField field);
    void VisitLibraryFunction(SFLibraryFunction function);
    void VisitLibraryTable(SFLibraryTable table);

    void VisitClass(SFClass @class);
    void VisitClassField(SFClassField field);
    void VisitClassMethod(SFClassMethod method);
    void VisitClassOperator(SFClassOperator @operator);

    void VisitReturnValue(SFReturnValue value);
    void VisitParameter(SFParameter parameter);
    void VisitFunctionOverload(SFFunctionOverload overload);
}
