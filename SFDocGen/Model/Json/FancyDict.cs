namespace SFDocGen.Model.Json;

public class FancyDict<T>
{
    public Dictionary<int, string> IndexMap { get; } = [];
    public Dictionary<string, T> Data { get; } = [];
    public int Count => Data.Count;

    public void Insert(int index, string key, T value)
    {
        IndexMap[index] = key;
        Data[key] = value;
    }

    public void Add(string key, T value)
    {
        IndexMap[Count] = key;
        Data[key] = value;
    }
}
