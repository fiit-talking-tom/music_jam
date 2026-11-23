using Serilog;

namespace MusicJam.Runtime;

public static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(".logs/start-host-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Runtime не смог запуститься");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); });
    }
}
