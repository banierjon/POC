using Microsoft.AspNetCore.SignalR.Client;
using Poc.Device.Services;
using Poc.Device.Utils;
using Shared.Models;

namespace Poc.Device.BackgroundService;

public class DispatcherBackgroundServices(
    IOutboxService outboxService,
    IConfiguration configuration,
    IRegistrationService registrationService,
    ILogger<DispatcherBackgroundServices> logger)
    : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger<DispatcherBackgroundServices> _logger = logger;
  
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await registrationService.CheckRegistrationAsync(Utility.GetX509CertCertificate.Thumbprint);
        await EnqueueLogEvent();
        await RunAsync(stoppingToken);
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var baseUri = configuration["Logging:HubEndpoint"];
        var hubConnection =  Utility.InitConnectionBuilder(baseUri);
      
        while (!cancellationToken.IsCancellationRequested)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await hubConnection.StartAsync(cancellationToken);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    continue;
                }
            }
            
            var unsent = await outboxService.GetUnsentAsync();
            if ( unsent.Count > 0)
            {
                await PublishAsSentAsync(cancellationToken, unsent, hubConnection);
            }
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
    
    private async Task PublishAsSentAsync(CancellationToken cancellationToken, List<LogMessage> unsent, HubConnection hubConnection)
    {
        foreach (var rec in unsent)
        {
            var log = new LogMessage
            {
                ClientId = rec.ClientId,
                Timestamp = rec.Timestamp,
                Level = rec.Level,
                Message = rec.Message
            };
            try
            {
                await hubConnection.InvokeAsync("ReceiveLog", log, cancellationToken);
                await outboxService.MarkAsSentAsync(rec.Id);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, ex.Message);
                Console.WriteLine($"Error sending log {rec.Id}: {ex.Message}");
                break;
            }
        }
    }
    
    private async Task EnqueueLogEvent()
    {
        var log = new LogMessage
        {
            ClientId = Utility.GetX509CertCertificate.Thumbprint,
            Timestamp = DateTime.UtcNow,
            Level = "INFO",
            Message = $"Test log - {DateTimeOffset.UtcNow}",
        };
        log.Sign = Utility.HmacSha256Generator($"ClientId={log.ClientId}&Timestamp={log.Timestamp}&Level={log.Level}");
        await outboxService.EnqueueAsync(log);
    }
}