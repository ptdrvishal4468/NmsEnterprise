using Nms.Domain.Enums;

namespace Nms.Application.Alerts.Dtos;

public record AlertRuleDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string Description,
    MetricType MetricType,
    ComparisonOperator Operator,
    decimal ThresholdValue,
    AlertSeverity Severity,
    bool IsEnabled,
    Guid? DeviceId,
    DateTime CreatedAtUtc);