namespace SFDocGen.Services;

public class UpdateScheduler(IConfiguration configuration, ILogger<UpdateScheduler> logger, IServiceProvider provider) : BackgroundService
{
    public static readonly TimeSpan DefaultDelay = TimeSpan.FromMinutes(30);
    protected TimeSpan FetchDelay { get; set; } = configuration.GetValue<TimeSpan?>("FetchDelay") ?? DefaultDelay;

    private readonly FetchService _fetchService = provider.GetRequiredService<FetchService>();
    private readonly ParserService _parserService = provider.GetRequiredService<ParserService>();
    private readonly LuaGenerator _luaGenerator = provider.GetRequiredService<LuaGenerator>();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting periodic documentation fetch.");
        logger.LogInformation("Fetching delay: {Delay}.", FetchDelay);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Performing scheduled update.");

            _fetchService.Fetch();
            _parserService.UpdateDocumentation();
            _luaGenerator.GenerateLuaDoc();

            await Task.Delay(FetchDelay, stoppingToken);
        }
    }
}
