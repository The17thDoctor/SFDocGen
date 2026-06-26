using System.ComponentModel;

namespace SFDocGen.Model.Abstraction;

/// <summary>
/// Indicates the documentation element can be on either or both Client and Server realms.
/// <br/>Example: Hook, Function, Field
/// </summary>
public interface IHasRealm
{
    /// <summary>
    /// The realm in which the element is found.
    /// </summary>
    public Realm Realm { get; set; }
}

public enum Realm
{
    [Description("Shared")]
    Shared,

    [Description("Server")]
    Server,

    [Description("Client")]
    Client
}