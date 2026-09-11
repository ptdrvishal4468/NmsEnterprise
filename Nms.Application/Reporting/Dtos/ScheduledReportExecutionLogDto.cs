using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class ScheduledReportExecutionLogDto
{
    public Guid Id { get; set; }
    public Guid ScheduledReportId { get; set; }
    public DateTime ExecutedAtUtc { get; set; }
    public ReportExecutionStatus Status { get; set; }
    public int RecordCount { get; set; }
    public string? ErrorMessage { get; set; }
}