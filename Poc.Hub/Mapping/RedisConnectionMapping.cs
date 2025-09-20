using StackExchange.Redis;

namespace Poc.Hub.Mapping;


public class RedisConnectionMapping(IConnectionMultiplexer redis) : IConnectionMapping
{
    private readonly IDatabase _redis = redis.GetDatabase();
    private const string Prefix = "client-connection:";

    public async Task Add(string clientId, string connectionId)
    {
        await _redis.StringSetAsync(Prefix + clientId, connectionId);
    }

    public async Task Remove(string clientId, string connectionId)
    {
        var existing = await _redis.StringGetAsync(Prefix + clientId);
        if (existing == connectionId)
        {
            await _redis.KeyDeleteAsync(Prefix + clientId);
        }
    }

    public async Task<string?> GetConnectionId(string clientId)
    {
        return await _redis.StringGetAsync(Prefix + clientId);
    }
}