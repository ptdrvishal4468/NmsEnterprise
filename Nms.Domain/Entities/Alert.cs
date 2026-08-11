using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class Alert : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid AlertRuleId { get; private set; }
    public Guid DeviceId { get; private set; }
    public MetricType MetricType { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public AlertState State { get; private set; }
    public decimal MetricValue { get; private set; }
    public decimal ThresholdValue { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime TriggeredAtUtc { get; private set; }
    public DateTime? LastOccurredAtUtc { get; private set; }
    public DateTime? AcknowledgedAtUtc { get; private set; }
    public string? AcknowledgedBy { get; private set; }
    public DateTime? SuppressedAtUtc { get; private set; }
    public string? SuppressedBy { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }

    public AlertRule? AlertRule { get; private set; }
    public Device? Device { get; private set; }

    private Alert() { }

    public Alert(
        Guid tenantId,
        Guid alertRuleId,
        Guid deviceId,
        MetricType metricType,
        AlertSeverity severity,
        decimal metricValue,
        decimal thresholdValue,
        string message)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AlertRuleId = alertRuleId;
        DeviceId = deviceId;
        MetricType = metricType;
        Severity = severity;
        State = AlertState.Active;
        MetricValue = metricValue;
        ThresholdValue = thresholdValue;
        Message = message;
        TriggeredAtUtc = DateTime.UtcNow;
        LastOccurredAtUtc = TriggeredAtUtc;
    }

    public void RecordRepeatedBreach(decimal metricValue)
    {
        MetricValue = metricValue;
        LastOccurredAtUtc = DateTime.UtcNow;
    }

    public void Acknowledge(string user)
    {
        if (State == AlertState.Resolved) return;
        State = AlertState.Acknowledged;
        AcknowledgedBy = user;
        AcknowledgedAtUtc = DateTime.UtcNow;
    }

    public void Suppress(string user)
    {
        if (State == AlertState.Resolved) return;
        State = AlertState.Suppressed;
        SuppressedBy = user;
        SuppressedAtUtc = DateTime.UtcNow;
    }

    public void Resolve()
    {
        State = AlertState.Resolved;
        ResolvedAtUtc = DateTime.UtcNow;
    }
}