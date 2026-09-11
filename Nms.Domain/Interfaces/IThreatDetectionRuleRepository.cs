using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IThreatDetectionRuleRepository : IGenericRepository<ThreatDetectionRule, Guid>
{
    Task<ThreatDetectionRule?> GetRuleByThreatTypeAsync(
        Guid tenantId,
        ThreatType threatType,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ThreatDetectionRule> Items, int TotalCount)> GetPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}