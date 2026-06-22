namespace SFDocGen.Model.Abstraction;

public interface IReturnsValue
{
    List<SFReturnValue> ReturnValues { get; set; }
}

public record SFReturnValue : IDocValue
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Types { get; set; } = [];

    public static SFReturnValue FromData(string name, string description)
    {
        SFReturnValue returnValue = new()
        {
            Name = name,
            Description = description
        };

        return returnValue;
    }
}