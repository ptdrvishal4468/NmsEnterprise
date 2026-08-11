using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class AlertRule : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public MetricType MetricType { get; private set; }
    public ComparisonOperator Operator { get; private set; }
    public decimal ThresholdValue { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public bool IsEnabled { get; private set; }
    public Guid? DeviceId { get; private set; } // Nullable: applies to all tenant devices if null

    private AlertRule() { }

    public AlertRule(
        Guid tenantId,
        string name,
        string description,
        MetricType metricType,
        ComparisonOperator op,
        decimal thresholdValue,
        AlertSeverity severity,
        Guid? deviceId = null)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        Description = description;
        MetricType = metricType;
        Operator = op;
        ThresholdValue = thresholdValue;
        Severity = severity;
        IsEnabled = true;
        DeviceId = deviceId;
    }

    public void Update(
        string name,
        string description,
        MetricType metricType,
        ComparisonOperator op,
        decimal thresholdValue,
        AlertSeverity severity,
        Guid? deviceId)
    {
        Name = name;
        Description = description;
        MetricType = metricType;
        Operator = op;
        ThresholdValue = thresholdValue;
        Severity = severity;
        DeviceId = deviceId;
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
}