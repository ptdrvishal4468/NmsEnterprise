using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IAlertRepository : IGenericRepository<Alert, Guid>
{
    Task<Alert?> GetActiveAlertByRuleAndDeviceAsync(Guid alertRuleId, Guid deviceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetActiveAlertsForDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Alert> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Guid? deviceId = null,
        AlertState? state = null,
        AlertSeverity? severity = null,
        CancellationToken cancellationToken = default);
}