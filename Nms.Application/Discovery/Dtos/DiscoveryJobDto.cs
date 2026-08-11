using Nms.Domain.Enums;

namespace Nms.Application.Discovery.Dtos;

public record DiscoveryJobDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string IpRange,
    DiscoveryJobStatus Status,
    int TotalTargets,
    int ProcessedTargets,
    int DiscoveredCount,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason,
    IReadOnlyList<DiscoveredDeviceCandidateDto> Candidates);