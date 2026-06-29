using SFDocGen.Core;

namespace SFDocGen.Services;

public class FetchService(IConfiguration configuration, IHttpClientFactory factory, ILogger<FetchService> logger, StorageManager storage)
{
    protected HttpClient FetchClient { get; } = factory.CreateClient();

    public void Fetch()
    {
        logger.LogInformation("Performing periodic fetch...");
        string? docUriString = configuration.GetValue<string>("FetchURI");

        if (docUriString is null)
        {
            logger.LogError("FetchURI not specified!");
            return;
        }

        Uri docUri = new(docUriString);
        logger.LogInformation("Fetching {URI}", docUri.ToString());
        try
        {
            using HttpResponseMessage response = FetchClient.GetAsync(docUri).Result;
            response.EnsureSuccessStatusCode();
            SaveFile(response.Content);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("Failed to fetch documentation: {Message}", ex.Message);
        }
    }

    protected void SaveFile(HttpContent content)
    {
        using Stream fileStream = File.OpenWrite(storage.Files.OriginalDoc);
        content.CopyTo(fileStream, null, CancellationToken.None);
        logger.LogInformation("Documentation saved.");
    }
}