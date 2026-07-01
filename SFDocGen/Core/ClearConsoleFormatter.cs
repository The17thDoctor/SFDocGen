using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace SFDocGen.Core;

public class ClearConsoleFormatter() : ConsoleFormatter(nameof(ClearConsoleFormatter))
{
    private const char ESCAPE = '\u001b';
    private static readonly bool _useAnsi = !Console.IsErrorRedirected && !Console.IsOutputRedirected;
    private static readonly int _padLength = Enum.GetValues<LogLevel>().Max(e => e.ToString().Length);

    private readonly Dictionary<LogLevel, string> ColorPalette = new()
    {
        { LogLevel.None, "38;5;8" },
        { LogLevel.Trace, "38;5;14" },
        { LogLevel.Debug, "38;5;7" },
        { LogLevel.Information, "38;5;2" },
        { LogLevel.Warning, "38;5;3" },
        { LogLevel.Error, "38;5;9" },
        { LogLevel.Critical, "38;5;1" }
    };

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        // Timestamp
        textWriter.Write($"[{DateTime.Now:yyyy/MM/dd}] [{DateTime.Now:HH:mm:ss}] ");

        // Log Level (with color)
        WriteANSI(textWriter, ColorPalette[logEntry.LogLevel]);
        textWriter.Write(logEntry.LogLevel);
        WriteANSI(textWriter, "0");

        // Padding
        textWriter.Write(new string(' ', _padLength - logEntry.LogLevel.ToString().Length));
        textWriter.Write(" | ");

        // Message
        textWriter.WriteLine(logEntry.State?.ToString());

        // Exception
        if (logEntry.Exception != null)
        {
            foreach (var line in logEntry.Exception.ToString().Split('\n'))
            {
                textWriter.Write(new string(' ', _padLength + 24));
                textWriter.Write(" | ");
                textWriter.WriteLine(line);
            }
        }
    }

    private static void WriteANSI(TextWriter textWriter, string sequence)
    {
        if (!_useAnsi) { return; }

        textWriter.Write(ESCAPE);
        textWriter.Write("[" + sequence);
        textWriter.Write('m');
    }
}
