using Model;
using System.Text.Json;

namespace SFDocGen.Services;

public class CorrecterService(ILogger<CorrecterService> logger)
{
    public const string CORRECTIONS_PATH = "Storage/docs-corrections.json";

    public void ApplyCorrection(SFDocRoot documentation)
    {
        string json = File.ReadAllText(CORRECTIONS_PATH);
        SFDocRoot? corrections = JsonSerializer.Deserialize<SFDocRoot>(json);

        if (corrections == null)
        {
            logger.LogWarning("Corrections file not found.");
            return;
        }
    }
}
