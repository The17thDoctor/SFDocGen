using Model;
using SFDocGen.Core;
using SFDocGen.Model;
using SFDocGen.Model.Abstraction;
using System.Text.Json;

namespace SFDocGen.Services;

public class CorrecterService(ILogger<CorrecterService> logger, StorageManager storage)
{
    public void ApplyCorrection(SFDocRoot documentation)
    {
        string json = File.Exists(storage.Files.CorrectionsFile) ? File.ReadAllText(storage.Files.CorrectionsFile) : "{}";
        SFDocRoot? corrections = JsonSerializer.Deserialize<SFDocRoot>(json);

        if (corrections == null)
        {
            logger.LogWarning("Corrections file not found.");
            return;
        }

        CorrecterExtensions.ApplyDict(documentation.Hooks, corrections.Hooks, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Libraries, corrections.Libraries, CorrecterExtensions.ApplyCorrection);
        CorrecterExtensions.ApplyDict(documentation.Classes, corrections.Classes, CorrecterExtensions.ApplyCorrection);
    }
}

file static class CorrecterExtensions
{
    public static void ApplyCorrection(this SFHook hook, SFHook correction)
    {
        hook.ApplyCorrection((SFDocElement)correction);
        hook.ApplyCorrection((IHasRealm)correction);
        hook.ApplyCorrection((IHasTypedParams)correction);
        hook.ApplyCorrection((IReturnsValue)correction);
    }

    public static void ApplyCorrection(this SFLibrary library, SFLibrary correction)
    {
        library.ApplyCorrection((SFDocElement)correction);
        library.ApplyCorrection((IHasRealm)correction);

        ApplyDict(library.Functions, correction.Functions, ApplyCorrection);
        ApplyDict(library.Fields, correction.Fields, ApplyCorrection);
        ApplyDict(library.Tables, correction.Tables, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFClass cl, SFClass correction)
    {
        cl.ApplyCorrection((SFDocElement)correction);
        cl.ApplyCorrection((IHasRealm)correction);

        ApplyDict(cl.Methods, correction.Methods, ApplyCorrection);
        ApplyDict(cl.Fields, correction.Fields, ApplyCorrection);
        ApplyDict(cl.Operators, correction.Operators, ApplyCorrection);
    }

    public static void ApplyCorrection(this SFClassField field, SFClassField correction)
    {
        field.ApplyCorrection((SFDocValue)correction);
        field.Type ??= correction.Type;
    }

    public static void ApplyCorrection(this SFClassOperator op, SFClassOperator correction)
    {
        op.ApplyCorrection((SFDocElement)correction);
        op.ApplyCorrection((IReturnsValue)correction);

        op.LeftOperand = correction.LeftOperand != string.Empty ? correction.LeftOperand : op.LeftOperand;
        op.RightOperand ??= correction.RightOperand;
    }

    public static void ApplyCorrection(this SFFunction function, SFFunction correction)
    {
        function.ApplyCorrection((SFDocElement)correction);
        function.ApplyCorrection((IHasRealm)correction);
        function.ApplyCorrection((IHasTypedParams)correction);
        function.ApplyCorrection((IReturnsValue)correction);
        function.ApplyCorrection((ICanBeGeneric)correction);

        function.Overloads = correction.Overloads;
    }

    public static void ApplyCorrection(this SFDirective directive, SFDirective correction)
    {
        directive.ApplyCorrection((SFDocElement)correction);
        directive.ApplyCorrection((IHasTypedParams)correction);
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

    public static void ApplyCorrection(this IReturnsValue returns, IReturnsValue correction, bool replace = false)
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
        value.Description ??= correction.Description;
    }

    public static void ApplyCorrection(this SFDocElement element, SFDocElement correction)
    {
        element.ApplyCorrection((SFDocValue)correction);
        element.Deprecated ??= correction.Deprecated;
        element.Usage ??= correction.Usage;
    }

    public static void ApplyDict<T>(Dictionary<string, T> dict, Dictionary<string, T> corrections, Action<T, T> correcter)
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
}