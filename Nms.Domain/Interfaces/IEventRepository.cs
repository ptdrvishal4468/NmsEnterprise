using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface IEventRepository : IGenericRepository<DeviceEvent, Guid>
{
    Task<(IReadOnlyList<DeviceEvent> Items, int TotalCount)> GetEventsPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        Guid? deviceId = null,
        EventCategory? category = null,
        EventSeverity? severity = null,
        string? correlationId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeviceEvent>> GetCorrelatedEventsAsync(
        Guid tenantId,
        string correlationId,
        CancellationToken cancellationToken = default);
}