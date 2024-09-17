using Serilog;

namespace Blogplace.Web;

public class Logger
{
    public static Serilog.Core.Logger CreateDefaultAppLogger()
    {
        var loggerConfigBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (currentEnvironment != null)
        {
            loggerConfigBuilder.AddJsonFile($"appsettings.{currentEnvironment}.json");
        }

        var loggerConfig = loggerConfigBuilder.Build();

        return new LoggerConfiguration()
            .ReadFrom.Configuration(loggerConfig)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }
}
