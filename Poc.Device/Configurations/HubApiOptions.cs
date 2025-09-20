namespace Poc.Device.Configurations;

public class HubApiOptions
{
    public Uri Endpoint { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}