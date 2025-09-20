using Microsoft.EntityFrameworkCore;
using Poc.Device.BackgroundService;
using Poc.Device.Configurations;
using Poc.Device.Repository;
using Poc.Device.Services;
using Poc.Device.Utils;

namespace Poc.Device;

public class Startup(IConfiguration config)
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = config.GetConnectionString("OutboxDatabase");
        services.AddDbContext<OutboxContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddHostedService<DispatcherBackgroundServices>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();
        services.AddLogging();
        services.AddHttpClient("RegistrationClient", client =>
        {
            var httpConfig = config.GetSection("HubApi").Get<HubApiOptions>();
            client.BaseAddress = httpConfig.Endpoint;
            client.Timeout = httpConfig.Timeout;
            client.DefaultRequestHeaders.Add("Client-Id", Utility.GetX509CertCertificate.Thumbprint);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(Utility.GetX509CertCertificate);
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            return handler;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(config.GetSection("AllowedCorsOrigins").Get<string[]>()!)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
        IHostApplicationLifetime applicationLifetime)
    {
        app.UseRouting();
        app.UseHealthChecks("/health");
        // app.UseCors();

        applicationLifetime.ApplicationStopped.Register(() =>
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        });
    }
}