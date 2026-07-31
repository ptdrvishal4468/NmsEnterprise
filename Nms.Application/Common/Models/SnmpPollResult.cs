namespace Nms.Application.Common.Models;

public class SnmpPollResult
{
    public Guid DeviceId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime PolledAtUtc { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> OidValues { get; set; } = new();
}