using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class ScheduledReport : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public ReportType ReportType { get; private set; }
    public ReportScheduleFrequency ScheduleFrequency { get; private set; }
    public ReportFormat OutputFormat { get; private set; }
    public string? FilterJson { get; private set; }
    public string RecipientEmail { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastRunUtc { get; private set; }
    public DateTime? NextRunUtc { get; private set; }

    // EF Core private constructor
    private ScheduledReport() { }

    public ScheduledReport(
        Guid id,
        Guid tenantId,
        string name,
        ReportType reportType,
        ReportScheduleFrequency scheduleFrequency,
        ReportFormat outputFormat,
        string recipientEmail,
        string? filterJson = null) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Report name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("Recipient email is required.", nameof(recipientEmail));

        TenantId = tenantId;
        Name = name;
        ReportType = reportType;
        ScheduleFrequency = scheduleFrequency;
        OutputFormat = outputFormat;
        RecipientEmail = recipientEmail;
        FilterJson = filterJson;

        CalculateNextRunTime();
    }

    public void Update(
        string name,
        ReportType reportType,
        ReportScheduleFrequency scheduleFrequency,
        ReportFormat outputFormat,
        string recipientEmail,
        string? filterJson)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Report name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("Recipient email is required.", nameof(recipientEmail));

        Name = name;
        ReportType = reportType;
        OutputFormat = outputFormat;
        RecipientEmail = recipientEmail;
        FilterJson = filterJson;

        if (ScheduleFrequency != scheduleFrequency)
        {
            ScheduleFrequency = scheduleFrequency;
            CalculateNextRunTime(); // Recalculate if frequency changes
        }
    }

    public void ToggleActive(bool isActive)
    {
        IsActive = isActive;
        if (IsActive)
        {
            CalculateNextRunTime();
        }
        else
        {
            NextRunUtc = null;
        }
    }

    public void RecordExecution(DateTime executionTimeUtc)
    {
        LastRunUtc = executionTimeUtc;
        if (IsActive)
        {
            CalculateNextRunTime();
        }
    }

    private void CalculateNextRunTime()
    {
        var now = DateTime.UtcNow;
        NextRunUtc = ScheduleFrequency switch
        {
            ReportScheduleFrequency.Daily => now.AddDays(1),
            ReportScheduleFrequency.Weekly => now.AddDays(7),
            ReportScheduleFrequency.Monthly => now.AddMonths(1),
            _ => throw new InvalidOperationException("Unknown schedule frequency.")
        };
    }
}