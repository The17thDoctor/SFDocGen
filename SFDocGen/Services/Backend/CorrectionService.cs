using SFDocGen.Core;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Services.Backend;

public class CorrectionService(ConfigManager configs)
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
        alias.ApplyCorrection<SFTypeAlias>(correction);
        alias.Types = correction.Types;
    }

    public static void ApplyCorrection(this SFLibrary library, SFLibrary correction)
    {
        library.ApplyCorrection<SFLibrary>(correction);
        ApplyDict(library.Functions, correction.Functions, ApplyCorrection);
        ApplyDict(library.Fields, correction.Fields, ApplyCorrection);
        ApplyDict(library.Tables, correction.Tables, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFClass @class, SFClass correction)
    {
        @class.ApplyCorrection<SFClass>(correction);
        ApplyDict(@class.Methods, correction.Methods, ApplyCorrection);
        ApplyDict(@class.Fields, correction.Fields, ApplyCorrection);
        ApplyDict(@class.Operators, correction.Operators, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFTable table, SFTable correction)
    {
        table.ApplyCorrection<SFTable>(correction);
        ApplyDict(table.Fields, correction.Fields, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFTableField field, SFTableField correction)
    {
        field.ApplyCorrection<SFTableField>(correction);
        field.Type ??= correction.Type;
        field.DefaultValue ??= correction.DefaultValue;
    }

    public static void ApplyCorrection(this SFClassField field, SFClassField correction)
    {
        field.ApplyCorrection<SFClassField>(correction);
        field.Type ??= correction.Type;
    }

    public static void ApplyCorrection(this SFClassOperator @operator, SFClassOperator correction)
    {
        @operator.ApplyCorrection<SFClassOperator>(correction);
        @operator.LeftOperand = correction.LeftOperand != string.Empty ? correction.LeftOperand : @operator.LeftOperand;
        @operator.RightOperand ??= correction.RightOperand;
    }

    public static void ApplyCorrection(this SFLibraryField field, SFLibraryField correction)
    {
        field.Value = correction.Value;
        field.Type = correction.Type;
    }

    public static void ApplyCorrection(this SFLibraryTable table, SFLibraryTable correction)
    {
        table.ApplyCorrection<SFLibraryTable>(correction);
    }

    public static void ApplyCorrection<T>(this SFFunction<T> function, SFFunction<T> correction) where T : SFDocValue
    {
        function.ApplyCorrection<SFFunction<T>>(correction);
        function.Overloads = correction.Overloads;
    }
    
    public static void ApplyCorrection(this SFParameter param, SFParameter correction)
    {
        param.ApplyCorrection<SFParameter>(correction);
        param.Types = correction.Types.Count > 0 ? correction.Types : param.Types;
    }

    public static void ApplyCorrection(this SFReturnValue ret, SFReturnValue correction)
    {
        ret.ApplyCorrection<SFReturnValue>(correction);
        ret.Types = correction.Types.Count > 0 ? correction.Types : ret.Types;
    }

    public static void ApplyCorrection<T>(this T value, T correction) where T : SFDocValue
    {
        value.Name ??= correction.Name;
        value.DocName ??= correction.DocName;
        value.Description ??= correction.Description;
        value.Deprecated ??= correction.Deprecated;
        value.Usage ??= correction.Usage;

        if (value is IHasRealm realmValue && correction is IHasRealm realmCorrection)
        {
            realmValue.Realm = realmCorrection.Realm;
        }

        if (value is ICanBeGeneric generic && correction is ICanBeGeneric genericCorrection)
        {
            generic.GenericTypes = genericCorrection.GenericTypes.Count > 0 ? genericCorrection.GenericTypes : generic.GenericTypes;
        }

        if (value is IReturnsValues returnsValues && correction is IReturnsValues returnsCorrection)
        {
            for (int i = 0; i < int.Max(returnsValues.ReturnValues.Count, returnsCorrection.ReturnValues.Count); i++)
            {
                SFReturnValue? ret = returnsValues.ReturnValues.ElementAtOrDefault(i);
                SFReturnValue? corr = returnsCorrection.ReturnValues.ElementAtOrDefault(i);

                if (ret != null && corr != null)
                {
                    ret.ApplyCorrection(corr);
                }
                else if (ret == null)
                {
                    returnsValues.ReturnValues.Insert(i, corr!);
                }
            }
        }

        if (value is IHasTypedParams typedParamsValue && correction is IHasTypedParams typedParamsCorrection)
        {
            for (int i = 0; i < int.Max(typedParamsValue.Parameters.Count, typedParamsCorrection.Parameters.Count); i++)
            {
                SFParameter? param = typedParamsValue.Parameters.ElementAtOrDefault(i);
                SFParameter? corr = typedParamsCorrection.Parameters.ElementAtOrDefault(i);

                if (param != null && corr != null)
                {
                    param.ApplyCorrection(corr);
                }
                else if (param == null)
                {
                    typedParamsValue.Parameters[i] = corr!;
                }
            }
        }
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