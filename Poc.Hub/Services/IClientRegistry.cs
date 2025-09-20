namespace Poc.Hub.Services;

public interface IClientRegistry
{
    Task<bool> RegisterClientAsync(string clientId, string subject);
    Task<bool> IsClientRegisteredAsync(string clientId);
    Task<bool> Register2ClientAsync(string clientId);
}