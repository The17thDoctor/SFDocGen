using Microsoft.Extensions.Logging.Console;
using SFDocGen.Core;
using SFDocGen.Services;

namespace SFDocGen;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Create docs storage directory
        Directory.CreateDirectory("Storage");

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
        app.MapOpenApi();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}