using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;
using MaterialSymbols = MudBlazor.FontIcons.MaterialSymbols;

namespace SFDocGen.Services.Frontend;

public class IconsHelper
{
    public readonly static string RealmClient = "<rect x=\"2\" y=\"2\" width=\"20\" height=\"20\" rx=\"5\" fill=\"#FFB500\"/>";
    public readonly static string RealmServer = "<rect x=\"2\" y=\"2\" width=\"20\" height=\"20\" rx=\"5\" fill=\"#0097FF\"/>";
    public readonly static string RealmShared = "<g stroke-width=\"4\"><path d=\"m7 2c-2.77 0-5 2.23-5 5v10c0 1.385 0.55719 2.6353 1.4609 3.5391l17.078-17.078c-0.90375-0.90375-2.1541-1.4609-3.5391-1.4609h-10z\" fill=\"#ffb500\" style=\"paint-order:fill markers stroke\"/><path d=\"m20.539 3.4609-17.078 17.078c0.90375 0.90375 2.1541 1.4609 3.5391 1.4609h10c2.77 0 5-2.23 5-5v-10c0-1.385-0.55719-2.6353-1.4609-3.5391z\" fill=\"#0097ff\" style=\"paint-order:fill markers stroke\"/></g>";

    private readonly static Dictionary<Type, string> _sfDocTypeMapping = new()
    {
        { typeof(SFClass), MaterialSymbols.Rounded.AccountTree },
        { typeof(SFClassField), MaterialSymbols.Rounded.ViewList },
        { typeof(SFClassMethod), MaterialSymbols.Rounded.DataObject },
        { typeof(SFClassOperator), MaterialSymbols.Rounded.Calculate },

        { typeof(SFLibrary), MaterialSymbols.Rounded.Book2 },
        { typeof(SFLibraryFunction), MaterialSymbols.Rounded.Function },
        { typeof(SFLibraryField), MaterialSymbols.Rounded.ViewList },
        { typeof(SFLibraryTable), MaterialSymbols.Rounded.DataArray },

        { typeof(SFDirective), MaterialSymbols.Rounded.Code },
        { typeof(SFHook), MaterialSymbols.Rounded.Bolt },
        { typeof(SFTable), MaterialSymbols.Rounded.Stack }
    };

    /// <summary>
    /// Returns the icon attributed to the given SFDocValue type.
    /// </summary>
    /// <param name="value">The value to get the icon of.</param>
    /// <param name="useRealm">If the value is subject to realm, whether to return the corresponding realm icon.</param>
    public static string SFDocIcon(SFDocValue value, bool useRealm = true)
    {
        if (useRealm && value is IHasRealm realmValue)
        {
            return RealmIcon(realmValue);
        }

        return SFDocIcon(value.GetType());
    }

    /// <summary>
    /// Returns the icon attributed to the given SFDocValue type.
    /// </summary>
    /// <param name="valueType">The value type to get the icon of.</param>
    public static string SFDocIcon(Type valueType)
    {
        return _sfDocTypeMapping.GetValueOrDefault(valueType, MaterialSymbols.Rounded.QuestionMark);
    }

    /// <summary>
    /// Returns the corresponding realm icon representing the value's applicable realms.
    /// </summary>
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
