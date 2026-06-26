namespace SFDocGen.Model.Abstraction;

public abstract record DocValue
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public object? this[string property]
    {
        get => GetType().GetProperty(property)?.GetValue(this);
        set => GetType().GetProperty(property)?.SetValue(this, value);
    }
    public abstract string ToLuaDoc();
}

public abstract record DocElement : DocValue
{
    public string? Deprecated { get; set; }
    public string? Usage { get; set; }
}