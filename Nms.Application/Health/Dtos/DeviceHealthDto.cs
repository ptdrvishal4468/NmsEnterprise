using Nms.Domain.Enums;

namespace Nms.Application.Health.Dtos;

public record DeviceHealthDto(
    Guid DeviceId,
    string DeviceName,
    double HealthScore,
    DeviceStatus Status,
    string SummaryReason,
    DateTime? LastSeenUtc,
    DateTime EvaluatedAtUtc);