using SFDocGen.Model.Abstraction;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SFDocGen.Model.Core;

public class ChildDictionary<TParent, TKey, TValue>(TParent parent) : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TParent : SFDocValue
    where TKey : notnull 
    where TValue : IChildObject<TParent>
{
    private readonly Dictionary<TKey, TValue> _internalDict = [];

    public TValue this[TKey key]
    {
        get => _internalDict[key];
        set
        {
            value.Parent = parent;
            _internalDict[key] = value;
        }
    }

    public ICollection<TKey> Keys => _internalDict.Keys;

    public ICollection<TValue> Values => _internalDict.Values;

    public int Count => _internalDict.Count;

    public bool IsReadOnly => false;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public void Add(TKey key, TValue value)
    {
        value.Parent = parent;
        _internalDict.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        item.Value.Parent = parent;
        _internalDict.Add(item.Key, item.Value);
    }

    public void Clear() => _internalDict.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item) => _internalDict.Contains(item);

    public bool ContainsKey(TKey key) => _internalDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(arrayIndex, array.Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_internalDict.Count, array.Length);

        int i = arrayIndex;
        foreach (var item in _internalDict)
        {
            array[i] = item;
            i++;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _internalDict.GetEnumerator();

    public bool Remove(TKey key) => _internalDict.Remove(key);

    public bool Remove(KeyValuePair<TKey, TValue> item) => _internalDict.Remove(item.Key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _internalDict.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
