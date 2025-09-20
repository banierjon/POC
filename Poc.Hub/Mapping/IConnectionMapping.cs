namespace Poc.Hub.Mapping;

public interface IConnectionMapping
{
    Task Add(string? clientId, string connectionId);
    Task Remove(string clientId, string connectionId);
    Task<string?> GetConnectionId(string clientId);
}