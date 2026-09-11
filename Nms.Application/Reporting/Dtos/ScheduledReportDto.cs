using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class ScheduledReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public ReportScheduleFrequency ScheduleFrequency { get; set; }
    public ReportFormat OutputFormat { get; set; }
    public string? FilterJson { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastRunUtc { get; set; }
    public DateTime? NextRunUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}