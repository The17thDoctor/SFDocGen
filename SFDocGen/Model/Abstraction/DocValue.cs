namespace SFDocGen.Model.Abstraction;

public abstract record DocValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public abstract string ToLuaDoc();
}

public abstract record DocElement : DocValue
{
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
}