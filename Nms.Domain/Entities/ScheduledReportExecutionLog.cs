using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ScheduledReportExecutionLog : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid ScheduledReportId { get; private set; }
    public ScheduledReport ScheduledReport { get; private set; } = null!;
    public DateTime ExecutedAtUtc { get; private set; }
    public ReportExecutionStatus Status { get; private set; }
    public int RecordCount { get; private set; }
    public string? ErrorMessage { get; private set; }

    // EF Core private constructor
    private ScheduledReportExecutionLog() { }

    public ScheduledReportExecutionLog(
        Guid id,
        Guid tenantId,
        Guid scheduledReportId,
        DateTime executedAtUtc,
        ReportExecutionStatus status,
        int recordCount,
        string? errorMessage = null) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        if (scheduledReportId == Guid.Empty)
            throw new ArgumentException("Scheduled Report ID is required.", nameof(scheduledReportId));

        TenantId = tenantId;
        ScheduledReportId = scheduledReportId;
        ExecutedAtUtc = executedAtUtc;
        Status = status;
        RecordCount = recordCount;
        ErrorMessage = errorMessage;
    }
}