namespace SFDocGen.Model;

/// <summary>
/// Represents an include to an external file.
/// </summary>
public record Dependency(string Name, Uri Uri);
