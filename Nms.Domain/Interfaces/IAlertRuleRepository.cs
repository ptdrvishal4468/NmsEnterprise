using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IAlertRuleRepository : IGenericRepository<AlertRule, Guid>
{
    Task<IEnumerable<AlertRule>> GetActiveRulesForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AlertRule> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId = null,
        CancellationToken cancellationToken = default);
}