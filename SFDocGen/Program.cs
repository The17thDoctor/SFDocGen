using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.Console;
using SFDocGen.Components;
using SFDocGen.Core;
using SFDocGen.Services;

namespace SFDocGen;

public class Program
{
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

        // Razor
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        // API & Lua Generation
        builder.Services.AddSingleton<StorageManager>();
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
        });
        app.UseAuthorization();
        app.MapControllers();

        // Razor
        app.UseAntiforgery();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        // Run
        app.Run();
    }
}