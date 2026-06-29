using Microsoft.Extensions.Logging.Console;
using SFDocGen.Core;
using SFDocGen.Services;

namespace SFDocGen;

public class Program
{
    public static void Main(string[] args)
    {
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
        builder.Services.AddOpenApi();

        builder.Services.AddHostedService<UpdateScheduler>();

        builder.Services.AddSingleton<StorageManager>();
        builder.Services.AddSingleton<FetchService>();
        builder.Services.AddSingleton<ParserService>();
        builder.Services.AddSingleton<LuaGenerator>();
        builder.Services.AddSingleton<CorrecterService>();

        builder.Services.AddHttpClient();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var storage = scope.ServiceProvider.GetRequiredService<StorageManager>();
            storage.CreateStorageFolder();
        }

        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}