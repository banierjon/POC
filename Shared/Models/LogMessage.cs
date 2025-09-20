namespace Shared.Models;
public class LogMessage
{
    public string ClientId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    
    public string Fingerprint { get; set; }
    public string Sign { get; set; }
    public long Id { get; set; }
}