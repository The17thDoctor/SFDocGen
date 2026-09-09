namespace SFDocGen.Blazor;
public enum ContentPlacement
{
    None,        // The child content will not be placed.
    Realm,       // The child content will be placed after the Realm text.
    Description, // The child content will be placed after the Description.
    Parameters,  // The child content will be placed after the parameters.
    ReturnValues,// The child content will be placed after the return values.
    Usage        // The child content will be placed after the Usage code block.
}