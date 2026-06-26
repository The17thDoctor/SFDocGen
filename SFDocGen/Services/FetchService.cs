namespace SFDocGen.Services;

public class FetchService(IConfiguration configuration, IHttpClientFactory factory, ILogger<FetchService> logger, ParserService parserService) : BackgroundService
{
    protected TimeSpan Delay { get; set; } = TimeSpan.FromMinutes(configuration.GetValue<uint>("FetchDelay"));
    protected HttpClient Client { get; } = factory.CreateClient();

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting periodic documentation fetch.");
        logger.LogInformation("Fetching delay: {Delay} minute(s).", Delay.Minutes);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (configuration.GetValue<bool>("DisableFetch") == true)
            {
                logger.LogInformation("Documentation fetch skipped.");
                parserService.UpdateModel();
                return;
            }

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
                using HttpResponseMessage response = await Client.GetAsync(docUri, stoppingToken);
                response.EnsureSuccessStatusCode();

                await SaveFileAsync(response.Content, stoppingToken);
                parserService.UpdateModel();
            }
            catch (TaskCanceledException) { return; }
            catch (HttpRequestException ex)
            {
                logger.LogError("Failed to fetch documentation: {Message}", ex.Message);
            }

            await Task.Delay(Delay, stoppingToken);
        }
    }

    protected async Task SaveFileAsync(HttpContent content, CancellationToken token)
    {
        using Stream fileStream = File.OpenWrite(ParserService.DOCS_PATH);
        content.CopyTo(fileStream, null, token);
        logger.LogInformation("Documentation saved.");
    }
}
