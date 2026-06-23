namespace SFDocGen.Model.Abstraction;

public interface IDocValue
{
    string Name { get; set; }
    string Description { get; set; }

    string ToLuaDoc();
}

public interface IDocElement : IDocValue
{
    string? Deprecated { get; set; }
    string? Usage { get; set; }
}