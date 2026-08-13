using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Interfaces;

public interface ISnmpTrapRepository : IGenericRepository<SnmpTrapMessage, Guid>
{
    Task<(IReadOnlyList<SnmpTrapMessage> Items, int TotalCount)> GetSnmpTrapsPagedAsync(
        Guid tenantId,
        Guid? deviceId = null,
        TrapSeverity? severity = null,
        string? sourceIpAddress = null,
        string? enterpriseOid = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}