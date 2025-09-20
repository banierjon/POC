using Shared;

namespace Poc.Device.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegistrationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<bool> CheckRegistrationAsync( string clientId)
    {
        var client = _httpClientFactory.CreateClient("RegistrationClient");
        var response = await client.PostAsync($"api/registration", null);
        return response.IsSuccessStatusCode;
    }


    public async Task<RegistrationResponse> RegisterAsync( string clientId)
    {
        var client = _httpClientFactory.CreateClient("RegistrationClient");
        var resp = await client.PostAsync($"api/registration", null);
        if (resp.IsSuccessStatusCode)
        {
           
            var rr = await resp.Content.ReadAsStringAsync();
            return new RegistrationResponse();
        }
        else
        {
            var err = await resp.Content.ReadAsStringAsync();
            return new RegistrationResponse { Success = false, Error = err };
        }
    }
}

public interface IRegistrationService
{
    Task<bool> CheckRegistrationAsync( string clientId);
    Task<RegistrationResponse> RegisterAsync( string clientId);
    
}