namespace Nms.Application.Auditing.Dtos;

public class ComplianceReportDto
{
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public int TotalAuditEvents { get; set; }
    public int TotalAuthenticationEvents { get; set; }
    public int FailedAuthenticationEvents { get; set; }
    public int TotalConfigurationChanges { get; set; }
    public int TotalSecurityViolations { get; set; }
    public IReadOnlyList<AuditLogDto> TopFailedOperations { get; set; } = new List<AuditLogDto>();
    public IReadOnlyList<AuditLogDto> CriticalEvents { get; set; } = new List<AuditLogDto>();
}