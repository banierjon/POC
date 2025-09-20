using Microsoft.EntityFrameworkCore;
using Poc.Device.Repository;
using Shared.Models;

namespace Poc.Device.Services;

public class OutboxService : IOutboxService
{
    private readonly OutboxContext _dbContext;
    
    public OutboxService(OutboxContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnqueueAsync(LogMessage log)
    {
            var outboxItem = new OutboxItem()
            {
                ClientId = log.ClientId,
                Timestamp = log.Timestamp,
                Level = log.Level,
                Message = log.Message,
                Sent = false,
                Sign = "S",
            };

            await _dbContext.OutboxItems.AddAsync(outboxItem);
            await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LogMessage>> GetUnsentAsync(int max = 50)
    {
        var unsentItems = await _dbContext.OutboxItems
            .Where(item => !item.Sent)
            .OrderBy(item => item.Id)
            .Take(max)
            .Select(item => new LogMessage
            {
                ClientId = item.ClientId,
                Timestamp = item.Timestamp,
                Level = item.Level,
                Message = item.Message,
                Sign  = item.Sign
            })
            .ToListAsync();
        
        return unsentItems;
    }

    public async Task MarkAsSentAsync(long id)
    {
        var item = await _dbContext.OutboxItems.FindAsync(id);
        if (item != null)
        {
            item.Sent = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}