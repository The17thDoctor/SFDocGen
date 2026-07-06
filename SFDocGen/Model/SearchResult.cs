using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Model;

public class SearchResult(SFDocValue value) : IComparable<SearchResult>
{
    public string Name => CraftName();
    public string Type => value.GetType().Name;
    public string? Description => value.Description;

    public int CompareTo(SearchResult? other)
    {
        if (other == null) return 0;
        return Name.Length - other.Name.Length;
    }

    private string CraftName()
    {
        string name = string.Empty;
        if (value is IChildObject<SFClass> classChild)
        {
            name += classChild.Parent.Name;
            name += ":";
        }
        else if (value is IChildObject<SFLibrary> libChild)
        {
            if (libChild.Parent.DocName != "_G")
            {
                name += libChild.Parent.DocName ?? libChild.Parent.Name;
                name += ".";
            }
        }

        name += value.Name;
        return name;
    }
}