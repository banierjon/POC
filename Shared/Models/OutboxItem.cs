namespace Shared.Models;

public class OutboxItem
{
    public long Id { get; set; }
    public string ClientId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public bool Sent { get; set; }
    public string Sign { get; set; }
}