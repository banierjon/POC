using System.Reflection;
using Serilog;

namespace Poc.Device;

public class Program
{
    public static void Main(string[] args)
    {
        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(c => c.UseStartup<Startup>())
            .UseSerilog((_, loggerConfiguration) =>
            {
                var logBasePath = config.GetSection("Logging:logBasePath").Value;
                loggerConfiguration.WriteTo.File(Path.Combine(serviceName, "log.txt"), 
                    rollingInterval: RollingInterval.Day, 
                    retainedFileCountLimit:50);
            })
            .Build().Run();
    }
}