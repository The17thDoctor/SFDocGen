using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SFDocGen.Model.Abstraction;

public interface IHasRealm
{
    public Realm Realm { get; }
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