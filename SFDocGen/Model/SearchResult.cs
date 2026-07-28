using SFDocGen.Model.Abstraction;

namespace SFDocGen.Model;

public class SearchResult : IComparable<SearchResult>
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }

    public static SearchResult FromValue(SFDocValue value)
    {
        return new()
        {
            Name = CraftName(value),
            Type = value.GetType().Name,
            Description = value.Description
        };
    }

    public int CompareTo(SearchResult? other)
    {
        if (other == null) return 0;
        return Name.Length - other.Name.Length;
    }

    private static string CraftName(SFDocValue value)
    {
        string name = string.Empty;
        if (value is IChildObject<SFClass> classChild)
        {
            name += classChild.Parent.Name;
            name += ":";
            name += value.Name;
            name += "()";
        }
        else if (value is IChildObject<SFLibrary> libChild)
        {
            if (libChild.Parent.DocName != "_G")
            {
                name += libChild.Parent.DocName ?? libChild.Parent.Name;
                name += ".";
            }

            name += value.Name;
            name += "()";
        }
        else if (value is IChildObject<SFTable> tableChild)
        {
            name += tableChild.Parent.Name;
            name += ".";
            name += value.Name;
        }

        return name;
    }
}