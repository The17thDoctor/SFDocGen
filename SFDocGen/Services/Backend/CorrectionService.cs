using SFDocGen.Core;
using SFDocGen.Model.Abstraction;
using SFDocGen.Model.Starfall;

namespace SFDocGen.Services.Backend;

public class CorrectionService(ConfigManager configs)
{
    public void ApplyCorrection(SFDocRoot documentation)
    {
        SFDocRoot corrections = configs.GetCorrections();
        CorrecterExtensions.ApplyDict(documentation.Aliases, corrections.Aliases, CorrecterExtensions.ApplyForAlias);
        CorrecterExtensions.ApplyDict(documentation.Hooks, corrections.Hooks, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Libraries, corrections.Libraries, CorrecterExtensions.ApplyForLib);
        CorrecterExtensions.ApplyDict(documentation.Classes, corrections.Classes, CorrecterExtensions.ApplyForClass);
        CorrecterExtensions.ApplyDict(documentation.Tables, corrections.Tables, CorrecterExtensions.ApplyForTable);
    }
}

file static class CorrecterExtensions
{
    public static void ApplyForAlias(this SFTypeAlias alias, SFTypeAlias correction)
    {
        alias.ApplyCorrection(correction);
        alias.Types = correction.Types;
    }

    public static void ApplyForLib(this SFLibrary library, SFLibrary correction)
    {
        library.ApplyCorrection(correction);
        ApplyDict(library.Functions, correction.Functions, ApplyForFunction);
        ApplyDict(library.Fields, correction.Fields, ApplyForLibField);
        ApplyDict(library.Tables, correction.Tables, ApplyCorrection);
    }

    public static void ApplyForClass(this SFClass @class, SFClass correction)
    {
        @class.ApplyCorrection(correction);
        ApplyDict(@class.Methods, correction.Methods, ApplyForFunction);
        ApplyDict(@class.Fields, correction.Fields, ApplyForClassField);
        ApplyDict(@class.Operators, correction.Operators, ApplyForClassOp);
    }

    public static void ApplyForTable(this SFTable table, SFTable correction)
    {
        table.ApplyCorrection(correction);
        ApplyDict(table.Fields, correction.Fields, ApplyForTableField);
    }

    public static void ApplyForTableField(this SFTableField field, SFTableField correction)
    {
        field.ApplyCorrection(correction);
        field.Type ??= correction.Type;
        field.DefaultValue ??= correction.DefaultValue;
    }

    public static void ApplyForClassField(this SFClassField field, SFClassField correction)
    {
        field.ApplyCorrection(correction);
        field.Type ??= correction.Type;
    }

    public static void ApplyForClassOp(this SFClassOperator @operator, SFClassOperator correction)
    {
        @operator.ApplyCorrection(correction);
        @operator.LeftOperand = correction.LeftOperand != string.Empty ? correction.LeftOperand : @operator.LeftOperand;
        @operator.RightOperand ??= correction.RightOperand;
    }

    public static void ApplyForLibField(this SFLibraryField field, SFLibraryField correction)
    {
        field.Value = correction.Value;
        field.Type = correction.Type;
    }

    public static void ApplyForFunction<T>(this SFFunction<T> function, SFFunction<T> correction) where T : SFDocValue
    {
        function.ApplyCorrection(correction);
        function.Overloads = correction.Overloads;
    }
    
    public static void ApplyForParam(this SFParameter param, SFParameter correction)
    {
        param.ApplyCorrection(correction);
        param.Types = correction.Types.Count > 0 ? correction.Types : param.Types;
    }

    public static void ApplyForRetValue(this SFReturnValue ret, SFReturnValue correction)
    {
        ret.ApplyCorrection(correction);
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
                    ret.ApplyForRetValue(corr);
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
                    param.ApplyForParam(corr);
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