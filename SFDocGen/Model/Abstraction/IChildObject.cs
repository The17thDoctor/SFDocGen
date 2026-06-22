namespace SFDocGen.Model.Abstraction;

public interface IChildObject<T>
{
    T Parent { get; }
}
