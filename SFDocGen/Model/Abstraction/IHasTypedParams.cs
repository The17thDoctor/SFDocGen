namespace SFDocGen.Model.Abstraction;

public interface IHasTypedParams
{
    List<SFParameter> Parameters { get; set; }
}

public record SFParameter : IDocValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];

    public static SFParameter FromData(string name, string description)
    {
        return new()
        {
            Name = name,
            Description = description
        };
    }
}