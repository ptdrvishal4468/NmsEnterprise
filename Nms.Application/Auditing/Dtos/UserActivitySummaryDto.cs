namespace Nms.Application.Auditing.Dtos;

public class UserActivitySummaryDto
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public int TotalActions { get; set; }
    public int SuccessfulActions { get; set; }
    public int FailedActions { get; set; }
    public DateTime? LastActiveUtc { get; set; }
    public IReadOnlyList<AuditLogDto> RecentActivities { get; set; } = new List<AuditLogDto>();
}