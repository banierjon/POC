using System.Collections.Concurrent;

namespace Poc.Hub.Services;

public class ClientRegistry : IClientRegistry
{
    private readonly ConcurrentDictionary<string, string> _clients = new ();
  
    public Task<bool> RegisterClientAsync(string clientId, string subject)
    {
        var added = _clients.TryAdd(clientId, subject);
        return Task.FromResult(added);
    }
  
    public Task<bool> IsClientRegisteredAsync(string clientId)
    {
        var exists = _clients.ContainsKey(clientId);
        return Task.FromResult(exists);
    }

    public Task<bool> Register2ClientAsync(string clientId)
    {
        var added = _clients.TryAdd(clientId, clientId);
        return Task.FromResult(added);
    }
}