using Shared.Models;

namespace Poc.Device.Services;

public interface IOutboxService
{
    Task EnqueueAsync(LogMessage log);
    Task<List<LogMessage>> GetUnsentAsync(int max = 50);
    Task MarkAsSentAsync(long id);
}