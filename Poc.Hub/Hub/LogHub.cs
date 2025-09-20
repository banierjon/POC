using Microsoft.AspNetCore.SignalR;
using Poc.Hub.Mapping;
using Shared.Models;

namespace Poc.Hub.Hub;

public class LogHub(IConnectionMapping connectionMapping, Logger<LogHub> logger) : Microsoft.AspNetCore.SignalR.Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        await connectionMapping.Add(httpContext?.Request.Headers["Client-Id"].ToString(), Context.ConnectionId);
        var user = Context.User;
        if (user?.Identity?.IsAuthenticated ?? false)
        {
            var clientId = user.FindFirst("clientId")?.Value;

            clientId ??= "default";
            if (!string.IsNullOrEmpty(clientId))
            {
                await connectionMapping.Add(clientId, Context.ConnectionId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientId = Context.GetHttpContext()!.Request.Headers["Client-Id"].ToString();
        if (!string.IsNullOrEmpty(clientId))
        {
            logger.LogInformation($"Client {clientId} disconnected");
            await connectionMapping.Remove(clientId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Task ReceiveLog(LogMessage log)
    {
        logger.LogInformation(log.Message);
        Console.WriteLine($"[{log.Timestamp}] {log.ClientId} - {log.Level} - {log.Message}");
        return Task.CompletedTask;
    }
}