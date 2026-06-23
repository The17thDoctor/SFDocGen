using SFDocGen.Services;

namespace SFDocGen;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Create docs storage directory
        Directory.CreateDirectory("Storage");

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddHostedService<FetchService>();
        builder.Services.AddSingleton<DocParserService>();
        builder.Services.AddHttpClient();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}