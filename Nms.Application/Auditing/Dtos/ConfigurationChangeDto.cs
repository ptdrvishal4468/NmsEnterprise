namespace Nms.Application.Auditing.Dtos;

public class ConfigurationChangeDto
{
    public long AuditLogId { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public DateTime TimestampUtc { get; set; }
}