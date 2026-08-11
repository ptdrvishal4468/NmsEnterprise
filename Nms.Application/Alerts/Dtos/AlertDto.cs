using Nms.Domain.Enums;

namespace Nms.Application.Alerts.Dtos;

public record AlertDto(
    Guid Id,
    Guid TenantId,
    Guid AlertRuleId,
    Guid DeviceId,
    MetricType MetricType,
    AlertSeverity Severity,
    AlertState State,
    decimal MetricValue,
    decimal ThresholdValue,
    string Message,
    DateTime TriggeredAtUtc,
    DateTime? LastOccurredAtUtc,
    DateTime? AcknowledgedAtUtc,
    string? AcknowledgedBy,
    DateTime? SuppressedAtUtc,
    string? SuppressedBy,
    DateTime? ResolvedAtUtc);