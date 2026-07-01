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

        foreach (var entry in corrections.Hooks)
        {
            if (!documentation.Hooks.TryGetValue(entry.Key, out var hook))
            {
                documentation.Hooks.Add(entry.Key, entry.Value);
                continue;
            }

            hook.ApplyCorrection(entry.Value);
        }

        foreach (var entry in corrections.Libraries)
        {
            if (!documentation.Libraries.TryGetValue(entry.Key, out var library))
            {
                documentation.Libraries.Add(entry.Key, entry.Value);
                continue;
            }

            library.ApplyCorrection(entry.Value);
        }
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

        foreach (var entry in correction.Functions)
        {
            if (!library.Functions.TryGetValue(entry.Key, out var function))
            {
                library.Functions.Add(entry.Key, entry.Value);
                continue;
            }

            function.ApplyCorrection(entry.Value);
        }

        foreach (var entry in correction.Fields)
        {
            if (!library.Fields.TryGetValue(entry.Key, out var field))
            {
                library.Fields.Add(entry.Key, entry.Value);
                continue;
            }

            field.ApplyCorrection(entry.Value);
        }

        foreach (var entry in correction.Tables)
        {
            if (!library.Tables.TryGetValue(entry.Key, out var table))
            {
                library.Tables.Add(entry.Key, entry.Value);
                continue;
            }

            table.ApplyCorrection(entry.Value);
        }
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
}