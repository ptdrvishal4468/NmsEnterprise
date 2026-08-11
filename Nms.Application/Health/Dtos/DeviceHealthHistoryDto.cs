using Nms.Domain.Enums;

namespace Nms.Application.Health.Dtos;

public record DeviceHealthHistoryDto(
    Guid Id,
    Guid DeviceId,
    double HealthScore,
    DeviceStatus Status,
    string Reason,
    DateTime TimestampUtc);