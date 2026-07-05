using SFDocGen.Core;
using SFDocGen.Model;

namespace SFDocGen.Services;

public class FetchService(IConfiguration configuration, IHttpClientFactory factory, ILogger<FetchService> logger, StorageManager storage)
{
    protected HttpClient FetchClient { get; } = factory.CreateClient();

    public void Fetch()
    {
        string? docUriString = configuration.GetValue<string>("FetchURI");

        if (docUriString is null)
        {
            logger.LogError("FetchURI not specified!");
            return;
        }

        // Fetch SFDoc
        Uri docUri = new(docUriString);
        FetchFile(docUri, storage.Files.OriginalDoc);

        // Fetch Dependencies
        if (!File.Exists(storage.Files.DependenciesManifest)) return;

        if (Directory.Exists(storage.Folders.DependenciesFolder)) Directory.Delete(storage.Folders.DependenciesFolder, true);
        Directory.CreateDirectory(storage.Folders.DependenciesFolder);

        Parallel.ForEach(storage.ReadDependencies(), dep =>
        {
            FetchFile(dep.Uri, Path.Combine(storage.Folders.DependenciesFolder, dep.Name));
        });
    }

    protected void FetchFile(Uri uri, string path)
    {
        logger.LogInformation("Fetching {URI}", uri);
        try
        {
            using HttpResponseMessage response = FetchClient.GetAsync(uri).Result;
            response.EnsureSuccessStatusCode();

            SaveFile(response.Content, path);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch documentation");
        }
    }

    protected void SaveFile(HttpContent content, string path)
    {
        using Stream fileStream = File.OpenWrite(path);
        content.CopyTo(fileStream, null, CancellationToken.None);
    }
}