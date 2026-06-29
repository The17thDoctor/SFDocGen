using SFDocGen.Model.Abstraction;
using System.Text;

namespace SFDocGen.Model;

/// <summary>
/// Represents a Starfall pre-processor directive.
/// <br/>Example: --@shared
/// </summary>
public record SFDirective : SFDocElement, IHasTypedParams
{
    public List<SFParameter> Parameters { get; set; } = [];

    public override string ToLuaDoc()
    {
        StringBuilder sb = new();
        
        if (Description != null)
        {
            sb.AppendLine("---" + Description.Replace("\n", "`\n---"));
        }

        if (Usage != null)
        {
            sb.AppendLine("---Usage:");
            sb.AppendLine("---```lua");
            sb.AppendLine("---" + Usage.Replace("\n", "`\n---"));
            sb.AppendLine("---```");
        }

        sb.Append($"---@directive {Name}");

        if (Parameters.Count > 0)
        {
            sb.Append(' ');
            sb.AppendJoin(" ", Parameters.Select(p => $"<{p.Name}>"));
        }

        return sb.ToString();
    }
}