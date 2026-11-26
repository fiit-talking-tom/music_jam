using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;
using Serilog;

namespace MusicJam.Studio;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(".logs/start-host-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            var host = CreateHostBuilder(args).Build();

            await using (var scope = host.Services.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await dbContext.Database.MigrateAsync();
            }

            await host.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Studio не смог запуститься");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder =>
            {
                webHostBuilder.UseStartup<Startup>();
            });
    }
}
