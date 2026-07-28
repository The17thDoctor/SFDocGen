using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.Console;
using MudBlazor.Services;
using SFDocGen.Components;
using SFDocGen.Core;
using SFDocGen.Services.Backend;

namespace SFDocGen;

public class Program
{
    private static List<string> Urls = [];

    public static void Main(string[] args)
    {
        /* ------- BUILDER ------- */
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.ConfigureHostOptions(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddConsoleFormatter<ClearConsoleFormatter, ConsoleFormatterOptions>();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = nameof(ClearConsoleFormatter);
        });

        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        builder.Services.AddOpenApi();

        // Blazor
        builder.Services.AddMudServices();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddHttpClient("APIClient", (provider, client) =>
        {
            string baseUrl = Urls[0];
            client.BaseAddress = new Uri(baseUrl + "/api/docs/");
        });

        // API & Lua Generation
        builder.Services.AddSingleton<StorageManager>();
        builder.Services.AddSingleton<ConfigManager>();

        builder.Services.AddSingleton<FetchService>();
        builder.Services.AddSingleton<ParserService>();
        builder.Services.AddSingleton<LuaGenerator>();
        builder.Services.AddSingleton<CorrecterService>();
        builder.Services.AddHostedService<UpdateScheduler>();

        // Proxy support
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });


        /* ------- APPLICATION ------- */
        var app = builder.Build();

        // Initialize Storage
        using (var scope = app.Services.CreateScope())
        {
            var storage = scope.ServiceProvider.GetRequiredService<StorageManager>();
            storage.CreateStorageFolder();
        }

        // Proxy
        app.UseForwardedHeaders();

        // API & Swagger
        app.UseStaticFiles();
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
            options.DocumentTitle = "SFDocGen API | Documentation";
            options.RoutePrefix = "api/swagger";
        });
        app.UseAuthorization();
        app.MapControllers();

        // Blazor
        app.UseAntiforgery();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.UseStatusCodePagesWithRedirects("/error/{0}");

        // Run
        app.Start();
        Urls = [.. app.Urls];
        app.WaitForShutdown();
    }
}