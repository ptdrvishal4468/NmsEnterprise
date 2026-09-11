using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Dtos;

public record FirmwareUpgradeRecommendationDto(
    Guid Id,
    Guid TenantId,
    Guid DeviceId,
    string? DeviceName,
    string CurrentVersion,
    string RecommendedVersion,
    RecommendationPriority Priority,
    string Reasoning,
    int AssociatedVulnerabilityCount,
    int CriticalVulnerabilityCount,
    bool IsApplied,
    DateTime CreatedAtUtc);