using SFDocGen.Core;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Services.Backend;

public class CorrecterService(ConfigManager configs)
{
    public void ApplyCorrection(SFDocRoot documentation)
    {
        SFDocRoot corrections = configs.GetCorrections();
        CorrecterExtensions.ApplyDict(documentation.Aliases, corrections.Aliases, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Hooks, corrections.Hooks, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Libraries, corrections.Libraries, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Classes, corrections.Classes, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Tables, corrections.Tables, CorrecterExtensions.ApplyCorrection);
    }
}

file static class CorrecterExtensions
{
    public static void ApplyCorrection(this SFTypeAlias alias, SFTypeAlias correction)
    {
        alias.ApplyCorrection((SFDocValue)correction);
        alias.Types = correction.Types;
    }

    public static void ApplyCorrection(this SFHook hook, SFHook correction)
    {
        hook.ApplyCorrection((SFDocValue)correction);
        hook.ApplyCorrection((IHasRealm)correction);
        hook.ApplyCorrection((IHasTypedParams)correction);
        hook.ApplyCorrection((IReturnsValues)correction);
    }

    public static void ApplyCorrection(this SFLibrary library, SFLibrary correction)
    {
        library.ApplyCorrection((SFDocValue)correction);
        library.ApplyCorrection((IHasRealm)correction);

        ApplyDictParent(library, library.Functions, correction.Functions, ApplyCorrection);
        ApplyDictParent(library, library.Fields, correction.Fields, ApplyCorrection);
        ApplyDictParent(library, library.Tables, correction.Tables, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFClass cl, SFClass correction)
    {
        cl.ApplyCorrection((SFDocValue)correction);
        cl.ApplyCorrection((IHasRealm)correction);

        ApplyDictParent(cl, cl.Methods, correction.Methods, ApplyCorrection);
        ApplyDictParent(cl, cl.Fields, correction.Fields, ApplyCorrection);
        ApplyDictParent(cl, cl.Operators, correction.Operators, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFTable table, SFTable correction)
    {
        table.ApplyCorrection((SFDocValue)correction);
        table.ApplyCorrection((IHasRealm)correction);

        ApplyDictParent(table, table.Fields, correction.Fields, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFDirective directive, SFDirective correction)
    {
        directive.ApplyCorrection((SFDocValue)correction);
        directive.ApplyCorrection((IHasTypedParams)correction);
    }

    public static void ApplyCorrection(this SFTableField field, SFTableField correction)
    {
        field.Type ??= correction.Type;
        field.DefaultValue ??= correction.DefaultValue;
    }

    public static void ApplyCorrection(this SFClassField field, SFClassField correction)
    {
        field.ApplyCorrection((SFDocValue)correction);
        field.Type ??= correction.Type;
    }

    public static void ApplyCorrection(this SFClassOperator op, SFClassOperator correction)
    {
        op.ApplyCorrection((SFDocValue)correction);
        op.ApplyCorrection((IReturnsValues)correction);

        op.LeftOperand = correction.LeftOperand != string.Empty ? correction.LeftOperand : op.LeftOperand;
        op.RightOperand ??= correction.RightOperand;
    }

    public static void ApplyCorrection(this SFLibraryField field, SFLibraryField correction)
    {
        field.Value = correction.Value;
        field.Type = correction.Type;
    }

    public static void ApplyCorrection(this SFLibraryTable table, SFLibraryTable correction)
    {
        table.ApplyCorrection((SFDocValue)correction);
        table.ApplyCorrection((IHasRealm)correction);
    }

    public static void ApplyCorrection<T>(this SFFunction<T> function, SFFunction<T> correction) where T : SFDocValue
    {
        function.ApplyCorrection((SFDocValue)correction);
        function.ApplyCorrection((IHasRealm)correction);
        function.ApplyCorrection((IHasTypedParams)correction);
        function.ApplyCorrection((IReturnsValues)correction);
        function.ApplyCorrection((ICanBeGeneric)correction);

        function.Overloads = correction.Overloads;
    }
    
    public static void ApplyCorrection(this SFParameter param, SFParameter correction)
    {
        param.ApplyCorrection((SFDocValue)correction);
        param.Types = correction.Types.Count > 0 ? correction.Types : param.Types;
    }

    public static void ApplyCorrection(this ICanBeGeneric generic, ICanBeGeneric correction)
    {
        generic.GenericTypes = correction.GenericTypes.Count > 0 ? correction.GenericTypes : generic.GenericTypes;
    }

    public static void ApplyCorrection(this SFReturnValue ret, SFReturnValue correction)
    {
        ret.ApplyCorrection((SFDocValue)correction);
        ret.Types = correction.Types.Count > 0 ? correction.Types : ret.Types;
    }

    public static void ApplyCorrection(this IHasRealm realmElement, IHasRealm correction)
    {
        realmElement.Realm = correction.Realm;
    }

    public static void ApplyCorrection(this IHasTypedParams paramElement, IHasTypedParams correction, bool replace = false)
    {
        if (replace)
        {
            paramElement.Parameters = correction.Parameters;
        }
        else
        {
            for (int i = 0; i < int.Max(paramElement.Parameters.Count, correction.Parameters.Count); i++)
            {
                SFParameter? param = paramElement.Parameters.ElementAtOrDefault(i);
                SFParameter? corr = correction.Parameters.ElementAtOrDefault(i);

                if (param != null && corr != null)
                {
                    param.ApplyCorrection(corr);
                }
                else if (param == null)
                {
                    paramElement.Parameters[i] = corr!;
                }
            }
        }
    }

    public static void ApplyCorrection(this IReturnsValues returns, IReturnsValues correction, bool replace = false)
    {
        if (replace)
        {
            returns.ReturnValues = correction.ReturnValues;
        }
        else
        {
            for (int i = 0; i < int.Max(returns.ReturnValues.Count, correction.ReturnValues.Count); i++)
            {
                SFReturnValue? ret = returns.ReturnValues.ElementAtOrDefault(i);
                SFReturnValue? corr = correction.ReturnValues.ElementAtOrDefault(i);

                if (ret != null && corr != null)
                {
                    ret.ApplyCorrection(corr);
                }
                else if (ret == null)
                {
                    returns.ReturnValues.Insert(i, corr!);
                }
            }
        }
    }

    public static void ApplyCorrection(this SFDocValue value, SFDocValue correction)
    {
        value.Name ??= correction.Name;
        value.DocName ??= correction.DocName;
        value.Description ??= correction.Description;
        value.Deprecated ??= correction.Deprecated;
        value.Usage ??= correction.Usage;
    }

    public static void ApplyDict<T>(IDictionary<string, T> dict, IDictionary<string, T> corrections, Action<T, T> correcter)
    {
        foreach (var kvp in corrections)
        {
            if (kvp.Value == null)
            {
                dict.Remove(kvp.Key);
                continue;
            }

            if (!dict.TryGetValue(kvp.Key, out T? value))
            {
                dict.Add(kvp.Key, kvp.Value);
                continue;
            }

            correcter(value, kvp.Value);
        }
    }

    public static void ApplyDictParent<TParent, TChild>(TParent parent, IDictionary<string, TChild> dict, IDictionary<string, TChild> corrections, Action<TChild, TChild> correcter)
        where TParent : SFDocValue
        where TChild : IChildObject<TParent>
    {
        foreach (var kvp in corrections)
        {
            if (kvp.Value == null)
            {
                dict.Remove(kvp.Key);
                continue;
            }

            if (!dict.TryGetValue(kvp.Key, out TChild? value))
            {
                kvp.Value.Parent = parent;
                dict.Add(kvp.Key, kvp.Value);
                continue;
            }

            correcter(value, kvp.Value);
        }
    }
}