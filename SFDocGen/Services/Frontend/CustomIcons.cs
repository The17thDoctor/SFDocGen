using SFDocGen.Model.Abstraction;

namespace SFDocGen.Services.Frontend;

public class CustomIcons
{
    public const string RealmClient = "<rect x=\"2\" y=\"2\" width=\"20\" height=\"20\" rx=\"5\" fill=\"#FFB500\"/>";
    public const string RealmServer = "<rect x=\"2\" y=\"2\" width=\"20\" height=\"20\" rx=\"5\" fill=\"#0097FF\"/>";
    public const string RealmShared = "<g stroke-width=\"4\"><path d=\"m7 2c-2.77 0-5 2.23-5 5v10c0 1.385 0.55719 2.6353 1.4609 3.5391l17.078-17.078c-0.90375-0.90375-2.1541-1.4609-3.5391-1.4609h-10z\" fill=\"#ffb500\" style=\"paint-order:fill markers stroke\"/><path d=\"m20.539 3.4609-17.078 17.078c0.90375 0.90375 2.1541 1.4609 3.5391 1.4609h10c2.77 0 5-2.23 5-5v-10c0-1.385-0.55719-2.6353-1.4609-3.5391z\" fill=\"#0097ff\" style=\"paint-order:fill markers stroke\"/></g>";

    public static string RealmIcon(IHasRealm realmValue)
    {
        return realmValue.Realm switch
        {
            Realm.Client => RealmClient,
            Realm.Server => RealmServer,
            Realm.Shared => RealmShared,
            _ => RealmShared
        };
    }
}
