using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IThreatIndicatorRepository : IGenericRepository<ThreatIndicator, Guid>
{
    Task<ThreatIndicator?> GetActiveIndicatorAsync(
        Guid tenantId,
        ThreatType type,
        string? sourceIp,
        Guid? targetDeviceId,
        string? targetUser,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ThreatIndicator> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        ThreatType? threatType = null,
        ThreatSeverity? severity = null,
        ThreatStatus? status = null,
        Guid? deviceId = null,
        CancellationToken cancellationToken = default);
}